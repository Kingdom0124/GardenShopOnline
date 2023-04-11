using GardenShopOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;

namespace GardenShopOnline.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            ShoppingCart.GetCart(HttpContext);
            return View();
        }

        [HttpGet]
        public ActionResult CategoryList()
        {
            var categories = db.Categories.Where(c => c.Status != 3).OrderByDescending(c => c.ID);
            return PartialView("_CategoryList", categories.ToList());
        }

        [HttpGet]
        public ActionResult ProductList(int? categoryId, int? typeId)
        {
            var links = from l in db.Products select l;

            if (categoryId != null)
            {
                links = links.Where(p => p.CategoryID == categoryId);
            }
            if (typeId != null)
            {
                links = links.Where(p => p.TypeID == typeId);
            }
            return PartialView("_ProductList", links.Where(c => c.Status == Constants.SHOW_STATUS).Take(10).OrderByDescending(c => c.ID));
        }

        [HttpGet]
        public ActionResult GetAdminSidebar()
        {
            string adminAccountId = UserManager.FindByEmail(Constants.ACCOUNT_BONSAIGARDEN).Id;
            ViewData["OrderCount"] = db.CustomerOrders.Count(c => c.Status == Constants.WAIT_FOR_CONFIRMATION);
            ViewData["CommentCount"] = db.CommentProducts.Count(c => c.Status == Constants.NEW_COMMENT);
            ViewData["ContactCount"] = db.Messages.Where(c => c.FromUserId != adminAccountId).GroupBy(c => c.FromUserId).Count(c => c.OrderByDescending(item => item.ID).FirstOrDefault().DateViewed == null);
            return PartialView("_AdminSidebar");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}