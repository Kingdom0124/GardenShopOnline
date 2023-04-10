using GardenShopOnline.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;

namespace GardenShopOnline.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    public class TypesController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        // GET: Types
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _TypeList()
        {
            var types = db.Types.OrderByDescending(c => c.ID);

            return PartialView(types.ToList());
        }

        public JsonResult Create_Type(string name_Type)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.Types.Where(c => c.Name == name_Type).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Type name already exists";
                }
                else
                {
                    Models.Type Type = new Models.Type
                    {
                        Name = name_Type,
                        Status = Constants.SHOW_STATUS
                    };
                    db.Types.Add(Type);
                    db.SaveChanges();
                    message = "Created successfully";
                    status = true;
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStatus_Type(Models.Type Types)
        {
            Models.Type Type = db.Types.Find(Types.ID);
            if (Type.Status == Constants.SHOW_STATUS)
            {
                Type.Status = Constants.HIDDEN_STATUS;
            }
            else
            {
                Type.Status = Constants.SHOW_STATUS;
            }
            db.Entry(Type).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Type", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FindType(int category_id)
        {
            Models.Type Types = db.Types.Find(category_id);
            var emp = new Models.Type
            {
                ID = category_id,
                Name = Types.Name
            };
            return Json(emp);
        }

        public JsonResult UpdateType(int Type_id, string name_Type)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.Types.Where(c => c.Name == name_Type).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Type name already exists";
                }
                else
                {
                    Models.Type Types = db.Types.Find(Type_id);
                    Types.Name = name_Type;
                    db.Entry(Types).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Record Saved Successfully ";
                    status = true;
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete_Type(Models.Type Type)
        {
            bool status = true;
            try
            {
                Models.Type Types = db.Types.Find(Type.ID);
                db.Types.Remove(Types);
                db.SaveChanges();
            }
            catch
            {
                status = false;
            }

            return Json(new { status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getType()
        {
            return Json(db.Types.Where(c => c.Status == Constants.SHOW_STATUS).OrderByDescending(c => c.ID).Select(x => new
            {
                TypeID = x.ID,
                TypeName = x.Name
            }).ToList(), JsonRequestBehavior.AllowGet);
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
