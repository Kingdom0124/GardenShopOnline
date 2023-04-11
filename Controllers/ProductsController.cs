using GardenShopOnline.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;

namespace GardenShopOnline.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();
        public ProductsController()
        {
            ViewBag.isCreate = false;
        }
        // GET: Products
        [Authorize(Roles = "Admin, Staff")]
        public ActionResult AdminIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string searchKey, int? categoryId, int? typeId)
        {
            var dbProduct = db.Products;
            ViewData["Category"] = new SelectList(db.Categories.ToList(), "ID", "Name", categoryId);
            ViewData["Type"] = new SelectList(db.Types.ToList(), "ID", "Name", typeId);
            ViewData["TotalCount"] = dbProduct.Where(x => x.Status == Constants.SHOW_STATUS).Count();
            var products = dbProduct.Where(x => x.Status == Constants.SHOW_STATUS && (string.IsNullOrEmpty(searchKey)
            || x.Name.ToLower().Contains(searchKey.ToLower()) || searchKey.ToLower().Contains(x.Name.ToLower()))
            && (!typeId.HasValue || x.TypeID == typeId)
            && (!categoryId.HasValue || x.CategoryID == categoryId));
            return View(products.ToList());
        }

        public ActionResult GetRelatedProducts(int productId, int typeId, int categoryId)
        {
            // Get related products list based on type and category
            return PartialView("_RelatedProducts", db.Products.Where(p => p.ID != productId && p.Status == Constants.SHOW_STATUS && (p.TypeID == typeId || p.CategoryID == categoryId)).ToList());
        }

        [Authorize(Roles = "Admin, Staff")]
        // Use PartialView not to reload page
        public ActionResult ProductList(int category_id, int type_id)
        {
            var links = from l in db.Products
                        where l.Category.Status != Constants.DELETED_STATUS && l.Type.Status != Constants.DELETED_STATUS
                        select l;

            if (category_id != -1)
            {
                links = links.Where(p => p.CategoryID == category_id);
            }
            if (type_id != -1)
            {
                links = links.Where(p => p.TypeID == type_id);
            }
            return PartialView(links.Where(c => c.Status != Constants.DELETED_STATUS).OrderByDescending(c => c.ID));

        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult EditStatus_Product(Product product)
        {
            Product products = db.Products.Find(product.ID);
            if (products.Status == Constants.SHOW_STATUS)
            {
                products.Status = Constants.HIDDEN_STATUS;
            }
            else
            {
                products.Status = Constants.SHOW_STATUS;
            }
            db.Entry(products).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Product", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult Delete_Product(Product product)
        {
            bool status = true;
            try
            {
                Product product1 = db.Products.Find(product.ID);
                product1.Status = Constants.DELETED_STATUS;
                db.Entry(product1).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                status = false;
            }

            return Json(new { status }, JsonRequestBehavior.AllowGet);
        }

        // GET: Products1/Create
        [Authorize(Roles = "Admin, Staff")]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name");
            ViewBag.isCreate = true;
            return View("Form");
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CategoryID,TypeID,Name,Description,Image,DateCreated,DateUpdate,Status,Quantity,Place")] Product product, HttpPostedFileBase[] Image, string priceProduct)
        {
            if (Image.Length <= 5)
            {
                if (ModelState.IsValid)
                {
                    decimal price = decimal.Parse(priceProduct.Replace(",", "").Replace(".", ""));
                    product.Status = Constants.SHOW_STATUS;
                    product.Price = price;
                    db.Products.Add(product);
                    foreach (var item in Image)
                    {
                        string filename = Path.GetFileName(item.FileName);
                        string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                        string extension = Path.GetExtension(item.FileName);

                        string path = Path.Combine(Server.MapPath("~/assets/images/"), _filename);

                        ImageProduct imageProduct = new ImageProduct
                        {
                            ProductID = product.ID,
                            Image = _filename
                        };
                        if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                        {
                            if (item.ContentLength <= 50000000)
                            {
                                db.ImageProducts.Add(imageProduct);
                                if (db.SaveChanges() > 0)
                                {
                                    item.SaveAs(path);
                                }
                            }
                            else
                            {
                                ViewBag.msg = "Img must be 50MB!";
                                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
                                ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
                                ViewBag.isCreate = true;
                                return View("Form", product);
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Fife not ling Aperp!";
                            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
                            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
                            ViewBag.isCreate = true;
                            return View("Form", product);
                        }
                    }
                    db.SaveChanges();
                    Session["notification"] = "Add succeeded!";
                    return RedirectToAction("AdminIndex");
                }
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            ViewBag.isCreate = true;
            return View("Form", product);
        }

        // GET: Products1/Edit/5
        [Authorize(Roles = "Admin, Staff")]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            return View("Form", product);
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CategoryID,TypeID,Name,Description,Price,Image,DateCreated,DateUpdate,Status,Quantity,Place")] Product product, HttpPostedFileBase[] file, string priceProduct)
        {
            if (ModelState.IsValid)
            {
                decimal price = decimal.Parse(priceProduct.Replace(",", "").Replace(".", ""));
                product.Price = price;
                if (file.Length > 0 && file[0] != null)
                {
                    var image = db.ImageProducts.Where(x => x.ProductID == product.ID);
                    foreach (var item in image)
                    {
                        db.ImageProducts.Remove(item);
                        string oldImgPath = Request.MapPath(item.Image);
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    foreach (var item in file)
                    {
                        string filename = Path.GetFileName(item.FileName);
                        string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                        string extension = Path.GetExtension(item.FileName);

                        string path = Path.Combine(Server.MapPath("~/assets/images/"), _filename);

                        ImageProduct imageProduct = new ImageProduct
                        {
                            ProductID = product.ID,
                            Image = _filename
                        };

                        if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                        {
                            if (item.ContentLength <= 50000000)
                            {
                                db.ImageProducts.Add(imageProduct);
                                item.SaveAs(path);
                            }
                            else
                            {
                                ViewBag.msg = "Pic must > 50MB!";
                                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
                                ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
                                ViewBag.isCreate = true;
                                return View("Form", product);
                            }
                        }
                        else
                        {
                            ViewBag.msg = "File not Apcet!";
                            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
                            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
                            ViewBag.isCreate = true;
                            return View("Form", product);
                        }
                    }
                    db.Entry(product).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        Session["notification"] = "Update succeeded!";
                        return RedirectToAction("AdminIndex");
                    }
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminIndex");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            return View(product);
        }

        // GET: Products/Details
        public ActionResult Details(int? id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            ViewData["CommentCount"] = db.CommentProducts.Where(c => c.ProductID == id && c.Status == Constants.HIDDEN_STATUS).Count();
            return View(product);
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult Comment_product(int product_id, string content)
        {
            CommentProduct comment = new CommentProduct
            {
                Content = content,
                ProductID = product_id,
                DateCreated = DateTime.Now,
                UserID = User.Identity.GetUserId(),
                Status = Constants.SHOW_STATUS
            };
            db.CommentProducts.Add(comment);
            db.SaveChanges();
            return Json("Comment_product", JsonRequestBehavior.AllowGet);
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
