using System.Linq;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using OnlineRetailStore.Mvc.Areas.Admin.Models;
using OnlineRetailStore.Mvc.Models;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        // GET: Admin/Categories
        public ActionResult Index()
        {
            var categories = Db.Categories.OrderBy(c => c.Name)
                .Select(c => new CategoryVm { Id = c.Id, Name = c.Name, Description = c.Description })
                .ToList();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(CategoryVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please provide a category name.";
                return RedirectToAction("Index");
            }

            try
            {
                Db.Categories.Add(new Category { Name = model.Name, Description = model.Description });
                Db.SaveChanges();
                TempData["Message"] = "Category added.";
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1062)
            {
                TempData["Message"] = "A category with that name already exists.";
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int id)
        {
            var category = Db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            return View(new CategoryVm { Id = category.Id, Name = category.Name, Description = category.Description });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var category = Db.Categories.Find(model.Id);
            if (category == null) return HttpNotFound();

            category.Name = model.Name;
            category.Description = model.Description;

            try
            {
                Db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1062)
            {
                ModelState.AddModelError(string.Empty, "A category with that name already exists.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = Db.Categories.Find(id);
            if (category != null)
            {
                Db.Categories.Remove(category);
                try
                {
                    Db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1451)
                {
                    TempData["Message"] = "Cannot delete a category that still has products assigned to it.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}
