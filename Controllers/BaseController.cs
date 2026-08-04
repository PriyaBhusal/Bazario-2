using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Models;

namespace OnlineRetailStore.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly AppDbContext Db = new AppDbContext();

        protected string CurrentUsername => Session["User"] as string;
        protected string CurrentUserEmail => Session["UserEmail"] as string;
        protected string CurrentRole => Session["Role"] as string;
        protected bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUsername);

        protected int CurrentUserId
        {
            get
            {
                if (Session["UserId"] != null) return System.Convert.ToInt32(Session["UserId"]);
                if (string.IsNullOrEmpty(CurrentUsername)) return 0;

                var id = Db.Users.Where(u => u.Username == CurrentUsername).Select(u => u.Id).FirstOrDefault();
                if (id != 0) Session["UserId"] = id;
                return id;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Db.Dispose();
            base.Dispose(disposing);
        }
    }
}
