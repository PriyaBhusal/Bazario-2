using System;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Filters;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.Services;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Controllers
{
    public class AccountController : BaseController
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            if (IsLoggedIn) return RedirectToAction("Index", "Home");
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        // Validates by email + password only, matching the legacy Login.aspx.cs behaviour
        // (the login form's "username" field has always actually held an email address).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = Db.Users.FirstOrDefault(u => u.Email == model.Email);
            var hash = PasswordHelper.HashSha256(model.Password);

            if (user == null || !string.Equals(user.PasswordHash, hash, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            Session["User"] = user.Username;
            Session["UserId"] = user.Id;
            Session["UserEmail"] = model.Email;
            if (!string.IsNullOrEmpty(user.Role)) Session["Role"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            if (IsLoggedIn) return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (Db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(string.Empty, "Username already exists");
                return View(model);
            }

            // New vendors require admin approval; other roles auto-approved.
            var role = string.Equals(model.Role, "Vendor", StringComparison.OrdinalIgnoreCase) ? "Vendor" : "User";
            var approved = !string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = PasswordHelper.HashSha256(model.Password),
                Role = role,
                IsApproved = approved,
                Phone = model.Phone,
                Address = model.Address
            };

            Db.Users.Add(user);
            Db.SaveChanges();

            Session["User"] = user.Username;
            Session["UserId"] = user.Id;
            Session["UserEmail"] = user.Email;
            Session["Role"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [RequireLogin]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [RequireLogin]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = Db.Users.FirstOrDefault(u => u.Username == CurrentUsername);
            if (user == null) return RedirectToAction("Login");

            if (!string.Equals(user.PasswordHash, PasswordHelper.HashSha256(model.OldPassword), StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect");
                return View(model);
            }

            user.PasswordHash = PasswordHelper.HashSha256(model.NewPassword);
            Db.SaveChanges();

            TempData["Message"] = "Password updated successfully";
            return RedirectToAction("Index", "Home");
        }
    }
}
