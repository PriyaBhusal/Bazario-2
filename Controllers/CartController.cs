using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;

namespace OnlineRetailStore.Mvc.Controllers
{
    [RequireLogin]
    public class CartController : BaseController
    {
        // GET: /Cart
        public ActionResult Index()
        {
            var cart = CartService.GetCart(Session);
            ViewBag.Total = MoneyHelper.Format(CartService.Total(Session));
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Increment(int productId)
        {
            CartService.Increment(Session, productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decrement(int productId)
        {
            CartService.Decrement(Session, productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int productId)
        {
            CartService.Remove(Session, productId);
            return RedirectToAction("Index");
        }

        // Reserves stock for every cart line inside a transaction (aborting the whole
        // checkout if anything is short), creates a Pending order, then hands off to
        // EsewaController to build the eSewa redirect. Mirrors Cart.aspx.cs's
        // btnCheckout_Click exactly.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            var cart = CartService.GetCart(Session);
            if (cart.Count == 0)
            {
                TempData["CartMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            int userId = CurrentUserId;
            if (userId == 0) return RedirectToAction("Login", "Account");

            decimal total = cart.Sum(c => c.LineTotal);
            int orderId;

            using (var tx = Db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in cart)
                    {
                        int affected = Db.Database.ExecuteSqlCommand(
                            "UPDATE products SET stock = stock - {0} WHERE id = {1} AND stock >= {0}",
                            item.Quantity, item.ProductId);

                        if (affected == 0)
                        {
                            tx.Rollback();
                            TempData["CartMessage"] = $"Not enough stock for {item.Name}. Please update your cart.";
                            return RedirectToAction("Index");
                        }
                    }

                    var user = Db.Users.Find(userId);

                    var order = new Order
                    {
                        UserId = userId,
                        Status = "Pending",
                        Total = total,
                        ShippingAddress = user?.Address,
                        ShippingPhone = user?.Phone
                    };
                    Db.Orders.Add(order);
                    Db.SaveChanges();

                    foreach (var item in cart)
                    {
                        Db.OrderItems.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            ProductName = item.Name,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });
                    }
                    Db.SaveChanges();

                    tx.Commit();
                    orderId = order.Id;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            // Order + items are committed with stock reserved, but the order stays Pending
            // until eSewa confirms payment - EsewaController.Pay builds the signed
            // redirect, and Success/Failure clear the cart / notify / restock.
            return RedirectToAction("Pay", "Esewa", new { order = orderId });
        }
    }
}
