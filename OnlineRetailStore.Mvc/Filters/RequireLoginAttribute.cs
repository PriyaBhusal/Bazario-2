using System.Web.Mvc;

namespace OnlineRetailStore.Mvc.Filters
{
    /// <summary>Redirects to /Account/Login when Session["User"] is empty — the MVC equivalent
    /// of the legacy app's `if (Session["User"] == null) Response.Redirect("Login.aspx")` guard
    /// repeated at the top of Cart.aspx.cs, MyOrders.aspx.cs, ManageProfile.aspx.cs, etc.</summary>
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var username = filterContext.HttpContext.Session?["User"] as string;
            if (string.IsNullOrEmpty(username))
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
