using System;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Areas.Admin.Models;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        // GET: Admin/Users
        // Lists vendor accounts only, matching the legacy ManageUsers.aspx page (which is
        // really vendor approval, despite the generic name).
        public ActionResult Index()
        {
            var vendors = Db.Users
                .Where(u => u.Role == "Vendor")
                .OrderBy(u => u.Id)
                .Select(u => new VendorUserVm
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    IsApproved = u.IsApproved,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            return View(vendors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id) => SetApproval(id, true);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(int id) => SetApproval(id, false);

        private ActionResult SetApproval(int id, bool approved)
        {
            var user = Db.Users.FirstOrDefault(u => u.Id == id && u.Role == "Vendor");
            if (user != null)
            {
                user.IsApproved = approved;
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Db.Users.Find(id);
            if (user != null)
            {
                if (string.Equals(user.Username, CurrentUsername, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "You cannot delete your own account while logged in.";
                    return RedirectToAction("Index");
                }

                Db.Users.Remove(user);
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Users/AllUsers
        // Lists every account so an existing admin can promote someone to Admin.
        public ActionResult AllUsers()
        {
            var users = Db.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserAdminVm
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeAdmin(int id)
        {
            var user = Db.Users.Find(id);
            if (user != null && !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                user.Role = "Admin";
                user.IsApproved = true;
                Db.SaveChanges();
            }

            return RedirectToAction("AllUsers");
        }
    }
}
