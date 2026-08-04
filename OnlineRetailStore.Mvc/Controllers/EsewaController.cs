using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;

namespace OnlineRetailStore.Mvc.Controllers
{
    public class EsewaController : BaseController
    {
        // GET: /Esewa/Pay?order=5
        // Hands the browser off to the eSewa sandbox for a single order's payment attempt.
        // Reached right after Cart.Checkout creates a Pending order with stock reserved.
        [RequireLogin]
        public ActionResult Pay(int order)
        {
            int userId = CurrentUserId;

            var dbOrder = Db.Orders.FirstOrDefault(o => o.Id == order && o.UserId == userId && o.Status == "Pending");
            if (dbOrder == null) return RedirectToAction("Index", "Orders");

            // Unique per attempt (not per order) so a user can retry a failed payment
            // without eSewa rejecting a reused transaction_uuid.
            var transactionUuid = $"{order}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            dbOrder.PaymentRef = transactionUuid;
            Db.SaveChanges();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/');
            var successUrl = baseUrl + Url.Action("Success", "Esewa");
            var failureUrl = baseUrl + Url.Action("Failure", "Esewa", new { order = dbOrder.Id });

            var fields = EsewaService.BuildPaymentRequest(dbOrder.Total, transactionUuid, successUrl, failureUrl);

            return View(fields);
        }

        // success_url target for eSewa ePay v2. eSewa appends a base64 `data` query param
        // describing the attempt - never trust it on its own (it's client-supplied), so this
        // also re-verifies the HMAC signature and re-confirms via eSewa's status API before
        // marking anything paid.
        public ActionResult Success(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return Fail("We couldn't read eSewa's response. If money was deducted, contact support with your order number.");
            }

            EsewaService.CallbackResult callback;
            try
            {
                callback = EsewaService.DecodeAndVerifyCallback(data);
            }
            catch
            {
                return Fail("eSewa's response could not be parsed. If money was deducted, contact support.");
            }

            if (!callback.SignatureValid)
            {
                return Fail("eSewa's response failed signature verification. If money was deducted, contact support.");
            }

            int orderId;
            try { orderId = int.Parse(callback.TransactionUuid.Split('-')[0]); }
            catch { return Fail("Could not match this payment to an order. Contact support."); }

            string liveStatus;
            try
            {
                liveStatus = EsewaService.CheckStatus(callback.TransactionUuid, callback.TotalAmount);
            }
            catch
            {
                return Fail("Could not confirm payment status with eSewa right now. Please check My Orders shortly, or contact support.");
            }

            if (liveStatus != "COMPLETE")
            {
                return Fail($"Payment was not completed (status: {liveStatus}).");
            }

            var order = Db.Orders.Include(o => o.User)
                .FirstOrDefault(o => o.Id == orderId && o.PaymentRef == callback.TransactionUuid && o.Status == "Pending");

            if (order == null)
            {
                return Fail("This order was already processed or could not be found.");
            }

            order.TransactionId = callback.TransactionCode;
            Db.SaveChanges();

            CartService.Clear(Session);

            NotificationService.NotifyAndEmail(
                Db, order.UserId, order.User?.Email,
                $"Order #{orderId} placed successfully — status: Pending.",
                $"Bazario order #{orderId} confirmed",
                $"Thanks for your order! Payment via eSewa was confirmed and order #{orderId} is now Pending.\nTotal: {MoneyHelper.Format(order.Total)}\n\nWe'll notify you as it moves through delivery.");

            return RedirectToAction("Index", "Orders", new { ok = 1 });
        }

        // failure_url target for eSewa ePay v2 - reached when the user cancels or the
        // payment is declined on eSewa's side. Cancels the Pending order it was created
        // for and restocks its items, the same way an admin-cancelled order does.
        public ActionResult Failure(int order)
        {
            if (order <= 0) return View();

            using (var tx = Db.Database.BeginTransaction())
            {
                try
                {
                    var dbOrder = Db.Orders.Find(order);
                    if (dbOrder == null || dbOrder.Status != "Pending")
                    {
                        tx.Rollback();
                        return View();
                    }

                    var lines = Db.OrderItems.Where(oi => oi.OrderId == order)
                        .Select(oi => new { oi.ProductId, oi.Quantity })
                        .ToList();

                    foreach (var line in lines)
                    {
                        Db.Database.ExecuteSqlCommand(
                            "UPDATE products SET stock = stock + {0} WHERE id = {1}",
                            line.Quantity, line.ProductId);
                    }

                    dbOrder.Status = "Cancelled";
                    Db.SaveChanges();

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            return View();
        }

        private ActionResult Fail(string message)
        {
            ViewBag.FailMessage = message;
            return View("PaymentNotConfirmed");
        }
    }
}
