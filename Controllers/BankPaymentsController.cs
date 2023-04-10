using GardenShopOnline.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;

namespace GardenShopOnline.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BankPaymentsController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();
        public BankPaymentsController()
        {
            ViewBag.isCreate = false;
        }

        // GET: BankPayments
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankPaymentList()
        {
            return PartialView(db.BankPayments.ToList().OrderByDescending(c => c.ID));

        }

        // GET: BankPayments/Create
        public ActionResult Create()
        {
            ViewBag.isCreate = true;
            return View("Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Bank,AccountNumber,AccountName,Image,Branch,Status")] BankPayment bankPayment, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                bankPayment.Status = Constants.SHOW_STATUS;
                string filename = Path.GetFileName(Image.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                string extension = Path.GetExtension(Image.FileName);

                string path = Path.Combine(Server.MapPath("~/assets/images/"), _filename);

                bankPayment.Image = _filename;
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (Image.ContentLength <= 4000000)
                    {
                        db.BankPayments.Add(bankPayment);

                        if (db.SaveChanges() > 0)
                        {
                            Image.SaveAs(path);

                        }
                    }
                    else
                    {
                        ViewBag.msg = "Hình ảnh phải lớn hơn hoặc bằng 4MB!";
                    }
                }
                else
                {
                    ViewBag.msg = "Định dạng file không hợp lệ!";
                }
                db.SaveChanges();
                Session["notification"] = "Add succeeded!";
                return RedirectToAction("Index");
            }
            ViewBag.isCreate = true;
            return View("Form", bankPayment);
        }

        // GET: BankPayments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankPayment bankPayment = db.BankPayments.Find(id);
            if (bankPayment == null)
            {
                return HttpNotFound();
            }
            return View("Form", bankPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Bank,AccountNumber,AccountName,Image,Branch,Status")] BankPayment bankPayment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                    string extension = Path.GetExtension(file.FileName);

                    string path = Path.Combine(Server.MapPath("~/assets/images/"), _filename);

                    bankPayment.Image = _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 4000000)
                        {
                            db.Entry(bankPayment).State = EntityState.Modified;
                            string oldImgPath = Request.MapPath(bankPayment.Image);

                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                Session["notification"] = "Update succeeded!";
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Hình ảnh phải lớn hơn hoặc bằng 4MB!";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Định dạng file không hợp lệ!";
                    }
                }
                db.Entry(bankPayment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankPayment);
        }

        public ActionResult Delete_BankPayment(BankPayment bankPayment)
        {
            bool status = true;
            try
            {
                BankPayment item = db.BankPayments.Find(bankPayment.ID);
                db.BankPayments.Remove(item);
                db.SaveChanges();
            }
            catch
            {
                status = false;
            }

            return Json(new { status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStatus_Bank(BankPayment bankPayment)
        {
            BankPayment bank = db.BankPayments.Find(bankPayment.ID);
            if (bank.Status == Constants.SHOW_STATUS)
            {
                bank.Status = Constants.HIDDEN_STATUS;
            }
            else
            {
                bank.Status = Constants.SHOW_STATUS;
            }
            db.Entry(bank).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Bank", JsonRequestBehavior.AllowGet);
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
