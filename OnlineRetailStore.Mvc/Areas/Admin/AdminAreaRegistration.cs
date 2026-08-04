using System.Web.Mvc;

namespace OnlineRetailStore.Mvc.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Users", action = "Index", id = UrlParameter.Optional },
                new[] { "OnlineRetailStore.Mvc.Areas.Admin.Controllers" }
            );
        }
    }
}
