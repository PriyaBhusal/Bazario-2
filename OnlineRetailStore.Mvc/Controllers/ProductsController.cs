using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    public class ProductsController : BaseController
    {
        // GET: /Products?cat=Apparel
        public ActionResult Index(string cat)
        {
            cat = cat ?? string.Empty;

            var model = new ProductIndexViewModel { IsLoggedIn = IsLoggedIn };

            model.Filters.Add(new ProductFilterViewModel { Label = "All", Category = "", Active = string.IsNullOrEmpty(cat) });
            foreach (var name in Db.Categories.OrderBy(c => c.Name).Select(c => c.Name))
            {
                model.Filters.Add(new ProductFilterViewModel { Label = name, Category = name, Active = name == cat });
            }

            var query = Db.Products.Include("Category").Include("Ratings").AsQueryable();
            if (!string.IsNullOrEmpty(cat)) query = query.Where(p => p.Category.Name == cat);

            var wishlisted = IsLoggedIn
                ? Db.Wishlists.Where(w => w.UserId == CurrentUserId).Select(w => w.ProductId).ToList()
                : new System.Collections.Generic.List<int>();

            model.Products = query
                .OrderBy(p => p.Id)
                .ToList()
                .Select(p => new ProductListItemViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    RatingCount = p.Ratings.Count,
                    AvgRating = p.Ratings.Count > 0 ? p.Ratings.Average(r => r.Score) : 0,
                    InWishlist = wishlisted.Contains(p.Id)
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int productId)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            var product = Db.Products.Find(productId);
            if (product != null)
            {
                CartService.AddToCart(Session, product.Id, product.Name, product.Price);
            }

            return RedirectToAction("Index", "Cart");
        }

        // GET: /Products/Details/5
        public ActionResult Details(int id)
        {
            var product = Db.Products.Include("Category").FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();

            var ratings = Db.Ratings.Where(r => r.ProductId == id).ToList();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                CategoryName = product.Category.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                RatingCount = ratings.Count,
                AvgRating = ratings.Count > 0 ? ratings.Average(r => r.Score) : 0,
                CanAddToCart = IsLoggedIn,
                InWishlist = IsLoggedIn && Db.Wishlists.Any(w => w.UserId == CurrentUserId && w.ProductId == id),
                Reviews = Db.Ratings
                    .Where(r => r.ProductId == id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .Select(r => new ReviewViewModel { Rating = r.Score, Comment = r.Comment, Username = r.User.Username })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCartFromDetails(int productId)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            var product = Db.Products.Find(productId);
            if (product == null) return RedirectToAction("Index");

            CartService.AddToCart(Session, product.Id, product.Name, product.Price);
            return RedirectToAction("Index", "Cart");
        }
    }
}
