using GardenShopOnline.Models;
using System.Linq;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;

namespace GardenShopOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        [HttpGet]
        public ActionResult Index()
        {
            ShoppingCart.GetCart(HttpContext);
            return View();
        }

        [HttpGet]
        public ActionResult CategoryList()
        {
            var categories = db.Categories.OrderByDescending(c => c.ID);
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
            ViewData["OrderCount"] = db.CustomerOrders.Count(c => c.Status == Constants.WAIT_FOR_CONFIRMATION);
            ViewData["CommentCount"] = db.CommentProducts.Count(c => c.Status == Constants.NEW_COMMENT);
            ViewData["ContactCount"] = db.Messages.Count(c => c.DateViewed == null);
            return PartialView("_AdminSidebar");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}