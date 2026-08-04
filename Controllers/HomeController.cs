using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly Dictionary<string, string> CategoryIcons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Apparel", "👕" },
            { "Gadgets", "🎧" },
            { "Tools", "🛠️" },
        };

        private static string CategoryIcon(string name)
        {
            return !string.IsNullOrEmpty(name) && CategoryIcons.TryGetValue(name, out var icon) ? icon : "🏷️";
        }

        // GET: /
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                IsLoggedIn = IsLoggedIn,
                StatCustomers = Db.Users.Count(u => u.Role == "User"),
                StatStock = Db.Products.Sum(p => (int?)p.Stock) ?? 0,
                StatDelivered = Db.Orders.Count(o => o.Status == "Delivered"),
            };

            model.Categories = Db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new { c.Name, c.Description })
                .ToList()
                .Select(c => new CategoryCardViewModel { Name = c.Name, Description = c.Description, Icon = CategoryIcon(c.Name) })
                .ToList();

            model.Testimonials = Db.Ratings
                .Where(r => r.Comment != null && r.Comment != "")
                .OrderByDescending(r => r.Score)
                .ThenByDescending(r => r.CreatedAt)
                .Take(3)
                .Select(r => new TestimonialViewModel { Rating = r.Score, Comment = r.Comment, Username = r.User.Username })
                .ToList();

            model.FeaturedProducts = Db.Products
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .Select(p => new ProductCardViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl
                })
                .ToList();

            if (IsLoggedIn && CurrentUserId != 0)
            {
                var notes = Db.Notifications
                    .Where(n => n.UserId == CurrentUserId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(10)
                    .ToList();

                model.Notifications = notes.Select(n => new NotificationViewModel { Message = n.Message, IsRead = n.IsRead, CreatedAt = n.CreatedAt }).ToList();
                model.UnreadCount = notes.Count(n => !n.IsRead);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkNotificationsRead()
        {
            if (IsLoggedIn && CurrentUserId != 0)
            {
                var unread = Db.Notifications.Where(n => n.UserId == CurrentUserId && !n.IsRead);
                foreach (var n in unread) n.IsRead = true;
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFeaturedToCart(int productId)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            var product = Db.Products.Find(productId);
            if (product != null)
            {
                CartService.AddToCart(Session, product.Id, product.Name, product.Price);
            }

            return RedirectToAction("Index", "Cart");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
