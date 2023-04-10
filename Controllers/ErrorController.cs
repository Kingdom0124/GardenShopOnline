using System.Web.Mvc;

namespace GardenShopOnline.Controllers
{
    public class ErrorController : Controller
    {
        // GET: NotFound
        public ActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}