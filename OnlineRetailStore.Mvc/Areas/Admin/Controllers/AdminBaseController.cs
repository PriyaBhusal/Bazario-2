using OnlineRetailStore.Mvc.Controllers;
using OnlineRetailStore.Mvc.Filters;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    [RequireRole("Admin")]
    public abstract class AdminBaseController : BaseController
    {
    }
}
