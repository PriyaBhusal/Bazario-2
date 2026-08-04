using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    [RequireLogin]
    public class OrdersController : BaseController
    {
        // GET: /Orders?ok=1  or  /Orders?rated=1
        public ActionResult Index(int? ok, int? rated)
        {
            if (ok == 1) ViewBag.Message = "Order placed successfully.";
            else if (rated == 1) ViewBag.Message = "Thanks for your feedback!";

            int userId = CurrentUserId;

            var orders = Db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList()
                .Select(o => new OrderStatusViewModel
                {
                    Id = o.Id,
                    Status = o.Status,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt
                })
                .ToList();

            foreach (var order in orders)
            {
                bool isDelivered = order.Status == "Delivered";

                var items = (from oi in Db.OrderItems
                             where oi.OrderId == order.Id
                             join r in Db.Ratings on new { oi.OrderId, oi.ProductId } equals new { r.OrderId, r.ProductId } into ratings
                             from r in ratings.DefaultIfEmpty()
                             select new OrderStatusItemViewModel
                             {
                                 OrderId = oi.OrderId,
                                 ProductId = oi.ProductId,
                                 ProductName = oi.ProductName,
                                 Quantity = oi.Quantity,
                                 ExistingRating = r == null ? (int?)null : r.Score
                             }).ToList();

                foreach (var item in items) item.ShowRate = isDelivered && item.ExistingRating == null;
                order.Items = items;
            }

            return View(orders);
        }

        // GET: /Orders/Rate?orderId=1&productId=2
        public ActionResult Rate(int orderId, int productId)
        {
            var model = LoadEligibility(orderId, productId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(EditReviewViewModel form)
        {
            var model = LoadEligibility(form.OrderId, form.ProductId);
            if (!model.Eligible) return View(model);

            if (!ModelState.IsValid)
            {
                model.Rating = form.Rating;
                model.Comment = form.Comment;
                return View(model);
            }

            int userId = CurrentUserId;
            var existing = Db.Ratings.FirstOrDefault(r => r.OrderId == form.OrderId && r.ProductId == form.ProductId && r.UserId == userId);
            if (existing != null)
            {
                existing.Score = form.Rating;
                existing.Comment = form.Comment;
            }
            else
            {
                Db.Ratings.Add(new Models.Rating
                {
                    OrderId = form.OrderId,
                    ProductId = form.ProductId,
                    UserId = userId,
                    Score = form.Rating,
                    Comment = form.Comment
                });
            }
            Db.SaveChanges();

            return RedirectToAction("Index", new { rated = 1 });
        }

        // Re-checked both on the initial GET and again at submit time - eligibility is:
        // the order belongs to the current user, is Delivered, and contains this product.
        private EditReviewViewModel LoadEligibility(int orderId, int productId)
        {
            int userId = CurrentUserId;

            var productName = (from o in Db.Orders
                                join oi in Db.OrderItems on o.Id equals oi.OrderId
                                where o.Id == orderId && o.UserId == userId && o.Status == "Delivered" && oi.ProductId == productId
                                select oi.ProductName).FirstOrDefault();

            return new EditReviewViewModel
            {
                OrderId = orderId,
                ProductId = productId,
                ProductName = productName,
                Eligible = productName != null
            };
        }
    }
}
