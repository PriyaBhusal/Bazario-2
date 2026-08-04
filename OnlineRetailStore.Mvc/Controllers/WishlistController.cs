using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    [RequireLogin]
    public class WishlistController : BaseController
    {
        // GET: /Wishlist
        public ActionResult Index()
        {
            var items = Db.Wishlists
                .Where(w => w.UserId == CurrentUserId)
                .Include("Product")
                .Include("Product.Category")
                .Include("Product.Ratings")
                .OrderByDescending(w => w.CreatedAt)
                .ToList()
                .Select(w => new ProductListItemViewModel
                {
                    ProductId = w.Product.Id,
                    Name = w.Product.Name,
                    Category = w.Product.Category.Name,
                    ShortDescription = w.Product.ShortDescription,
                    Price = w.Product.Price,
                    Stock = w.Product.Stock,
                    ImageUrl = w.Product.ImageUrl,
                    RatingCount = w.Product.Ratings.Count,
                    AvgRating = w.Product.Ratings.Count > 0 ? w.Product.Ratings.Average(r => r.Score) : 0,
                    InWishlist = true
                })
                .ToList();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int productId, string returnUrl)
        {
            bool exists = Db.Wishlists.Any(w => w.UserId == CurrentUserId && w.ProductId == productId);
            if (!exists)
            {
                Db.Wishlists.Add(new Wishlist { UserId = CurrentUserId, ProductId = productId });
                Db.SaveChanges();
            }
            return RedirectToLocalOrIndex(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int productId, string returnUrl)
        {
            var item = Db.Wishlists.FirstOrDefault(w => w.UserId == CurrentUserId && w.ProductId == productId);
            if (item != null)
            {
                Db.Wishlists.Remove(item);
                Db.SaveChanges();
            }
            return RedirectToLocalOrIndex(returnUrl);
        }

        private ActionResult RedirectToLocalOrIndex(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index");
        }
    }
}
