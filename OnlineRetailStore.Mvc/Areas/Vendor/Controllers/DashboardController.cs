using System.Linq;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using OnlineRetailStore.Mvc.Areas.Vendor.Models;
using OnlineRetailStore.Mvc.Controllers;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;

namespace OnlineRetailStore.Mvc.Areas.Vendor.Controllers
{
    [RequireRole("Vendor")]
    public class DashboardController : BaseController
    {
        private void PopulateCategories(object selected = null)
        {
            ViewBag.Categories = new SelectList(Db.Categories.OrderBy(c => c.Name).ToList(), "Id", "Name", selected);
        }

        // GET: Vendor/Dashboard
        public ActionResult Index()
        {
            int vendorId = CurrentUserId;
            if (vendorId == 0) return RedirectToAction("Login", "Account", new { area = "" });

            var vendor = Db.Users.Find(vendorId);
            bool approved = vendor != null && vendor.IsApproved;

            var model = new VendorDashboardVm { IsApproved = approved };
            if (!approved) return View(model);

            var products = Db.Products.Where(p => p.VendorId == vendorId).ToList();

            model.StatProducts = products.Count;
            model.StatStock = products.Sum(p => p.Stock);
            model.StatOutOfStock = products.Count(p => p.Stock == 0);
            model.StatValue = MoneyHelper.Format(products.Sum(p => p.Stock * p.Price));
            model.Products = products.OrderBy(p => p.Id).Select(p => new VendorProductVm
            {
                Id = p.Id,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl
            }).ToList();

            PopulateCategories();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(VendorProductVm model)
        {
            int vendorId = CurrentUserId;
            var vendor = Db.Users.Find(vendorId);
            if (vendor == null || !vendor.IsApproved)
            {
                TempData["Message"] = "Your account is not approved yet.";
                return RedirectToAction("Index");
            }

            if (!Db.Categories.Any())
            {
                TempData["Message"] = "No categories exist yet. Ask an admin to add one.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please fill in all required product fields.";
                return RedirectToAction("Index");
            }

            Db.Products.Add(new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                VendorId = vendorId,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? "~/Content/Images/placeholder.svg" : model.ImageUrl.Trim()
            });
            Db.SaveChanges();

            TempData["Message"] = "Product added.";
            return RedirectToAction("Index");
        }

        // GET: Vendor/Dashboard/Edit/5
        public ActionResult Edit(int id)
        {
            int vendorId = CurrentUserId;
            var product = Db.Products.FirstOrDefault(p => p.Id == id && p.VendorId == vendorId);
            if (product == null) return HttpNotFound();

            var model = new VendorProductVm
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };

            PopulateCategories(model.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorProductVm model)
        {
            int vendorId = CurrentUserId;

            if (!ModelState.IsValid)
            {
                PopulateCategories(model.CategoryId);
                return View(model);
            }

            var product = Db.Products.FirstOrDefault(p => p.Id == model.Id && p.VendorId == vendorId);
            if (product == null) return HttpNotFound();

            product.Name = model.Name;
            product.CategoryId = model.CategoryId;
            product.ShortDescription = model.ShortDescription;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.ImageUrl = model.ImageUrl;
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            int vendorId = CurrentUserId;
            var product = Db.Products.FirstOrDefault(p => p.Id == id && p.VendorId == vendorId);
            if (product != null)
            {
                Db.Products.Remove(product);
                try
                {
                    Db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1451)
                {
                    TempData["Message"] = "Cannot delete a product referenced by existing orders.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}
