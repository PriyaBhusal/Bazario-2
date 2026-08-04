using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Areas.Admin.Models;
using OnlineRetailStore.Mvc.Services;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = Db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToList()
                .Select(o => new AdminOrderVm
                {
                    Id = o.Id,
                    Username = o.User.Username,
                    Email = o.User.Email,
                    Status = o.Status,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt,
                    Items = string.Join(", ", o.OrderItems.Select(oi => $"{oi.ProductName} x{oi.Quantity}"))
                })
                .ToList();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OutForDelivery(int id) => SetStatus(id, "Sent out for delivery");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delivered(int id) => SetStatus(id, "Delivered");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            using (var tx = Db.Database.BeginTransaction())
            {
                try
                {
                    var order = Db.Orders.Find(id);
                    if (order == null || order.Status == "Cancelled" || order.Status == "Delivered")
                    {
                        tx.Rollback();
                        return RedirectToAction("Index");
                    }

                    var lines = Db.OrderItems.Where(oi => oi.OrderId == id)
                        .Select(oi => new { oi.ProductId, oi.Quantity })
                        .ToList();

                    foreach (var line in lines)
                    {
                        Db.Database.ExecuteSqlCommand(
                            "UPDATE products SET stock = stock + {0} WHERE id = {1}",
                            line.Quantity, line.ProductId);
                    }

                    order.Status = "Cancelled";
                    Db.SaveChanges();
                    tx.Commit();

                    NotifyOwner(id, "Cancelled");
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        private ActionResult SetStatus(int id, string newStatus)
        {
            var order = Db.Orders.Find(id);
            if (order != null)
            {
                order.Status = newStatus;
                Db.SaveChanges();
                NotifyOwner(id, newStatus);
            }

            return RedirectToAction("Index");
        }

        private void NotifyOwner(int orderId, string newStatus)
        {
            var order = Db.Orders.Include(o => o.User).FirstOrDefault(o => o.Id == orderId);
            if (order == null) return;

            NotificationService.NotifyAndEmail(
                Db, order.UserId, order.User?.Email,
                $"Order #{orderId} is now {newStatus}.",
                $"Bazario order #{orderId} update",
                $"Your order #{orderId} status changed to: {newStatus}.");
        }
    }
}
