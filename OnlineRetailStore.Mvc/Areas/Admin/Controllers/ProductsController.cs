using System.Linq;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using OnlineRetailStore.Mvc.Areas.Admin.Models;
using OnlineRetailStore.Mvc.Models;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private void PopulateCategories(object selected = null)
        {
            ViewBag.Categories = new SelectList(Db.Categories.OrderBy(c => c.Name).ToList(), "Id", "Name", selected);
        }

        // GET: Admin/Products
        public ActionResult Index()
        {
            var products = Db.Products
                .Select(p => new ProductAdminVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    OwnerName = p.Vendor != null ? p.Vendor.Username : "Store"
                })
                .OrderBy(p => p.Id)
                .ToList();

            PopulateCategories();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductAdminVm model)
        {
            if (!Db.Categories.Any())
            {
                TempData["Message"] = "Create a category first.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please fill in all required product fields.";
                return RedirectToAction("Index");
            }

            var product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? "~/Content/Images/placeholder.svg" : model.ImageUrl.Trim()
            };
            Db.Products.Add(product);
            Db.SaveChanges();

            TempData["Message"] = "Product added.";
            return RedirectToAction("Index");
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int id)
        {
            var product = Db.Products.Find(id);
            if (product == null) return HttpNotFound();

            var model = new ProductAdminVm
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
        public ActionResult Edit(ProductAdminVm model)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories(model.CategoryId);
                return View(model);
            }

            var product = Db.Products.Find(model.Id);
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
            var product = Db.Products.Find(id);
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
