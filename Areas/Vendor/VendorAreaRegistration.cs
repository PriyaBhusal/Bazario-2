using System.Web.Mvc;

namespace OnlineRetailStore.Mvc.Areas.Vendor
{
    public class VendorAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Vendor";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Vendor_default",
                "Vendor/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                new[] { "OnlineRetailStore.Mvc.Areas.Vendor.Controllers" }
            );
        }
    }
}
