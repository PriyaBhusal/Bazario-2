using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    [RequireLogin]
    public class ProfileController : BaseController
    {
        // GET: /Profile
        public ActionResult Index()
        {
            var user = Db.Users.FirstOrDefault(u => u.Username == CurrentUsername);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role ?? "User",
                MemberSince = user.CreatedAt.ToString("MMMM d, yyyy")
            };

            return View(model);
        }

        // POST: /Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileViewModel model)
        {
            var user = Db.Users.FirstOrDefault(u => u.Username == CurrentUsername);
            if (user == null) return RedirectToAction("Login", "Account");

            model.Id = user.Id;
            model.Role = user.Role ?? "User";
            model.MemberSince = user.CreatedAt.ToString("MMMM d, yyyy");

            if (!ModelState.IsValid) return View(model);

            if (Db.Users.Any(u => u.Username == model.Username && u.Id != user.Id))
            {
                ModelState.AddModelError(string.Empty, "That username is already taken.");
                return View(model);
            }

            if (Db.Users.Any(u => u.Email == model.Email && u.Id != user.Id))
            {
                ModelState.AddModelError(string.Empty, "That email is already registered.");
                return View(model);
            }

            user.Username = model.Username;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            Db.SaveChanges();

            Session["User"] = user.Username;
            Session["UserEmail"] = user.Email;

            ViewBag.SuccessMessage = "Profile updated successfully.";
            return View(model);
        }
    }
}
