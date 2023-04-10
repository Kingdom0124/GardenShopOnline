using GardenShopOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GardenShopOnline.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        // GET: Statistics/Order
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            var countList = new List<Tuple<int, int>>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var orderCount = db.CustomerOrders.Count(o => DbFunctions.TruncateTime(o.DateCreated) == date);
                var accountCount = db.AspNetUsers.Count(a => DbFunctions.TruncateTime(a.DateCreated) == date && !a.AspNetRoles.Any());
                countList.Add(Tuple.Create(orderCount, accountCount));
            }
            return Json(countList, JsonRequestBehavior.AllowGet);
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