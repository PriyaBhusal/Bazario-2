using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineRetailStore.Mvc
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Matches the legacy app's RouteConfig, which mapped bare "/admin" straight to
            // the vendor-approval page (ManageUsers.aspx) without a trailing-slash redirect loop.
            routes.MapRoute(
                name: "AdminShortcut",
                url: "admin",
                defaults: new { area = "Admin", controller = "Users", action = "Index" },
                namespaces: new[] { "OnlineRetailStore.Mvc.Areas.Admin.Controllers" }
            ).DataTokens["area"] = "Admin";

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineRetailStore.Mvc.Controllers" }
            );
        }
    }
}
