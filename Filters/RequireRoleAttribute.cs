using System;
using System.Web.Mvc;

namespace OnlineRetailStore.Mvc.Filters
{
    /// <summary>Redirects to the home page when Session["Role"] doesn't match — the MVC
    /// equivalent of the role checks at the top of Admin/*.aspx.cs and Vendor/Dashboard.aspx.cs.</summary>
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public RequireRoleAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = filterContext.HttpContext.Session?["Role"] as string;
            if (!string.Equals(role, _role, StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
