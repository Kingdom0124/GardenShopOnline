using GardenShopOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;


namespace GardenShopOnline.Controllers
{
    [Authorize]
    public class CustomerOrdersController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        public CustomerOrdersController()
        {
        }

        public CustomerOrdersController(ApplicationUserManager userManager)
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

        // GET: CustomerOrders
        [Authorize(Roles = "Admin, Staff")]
        public ActionResult Index()
        {
            Session["pills-create"] = "active";
            Session["pills-confirm"] = "";
            Session["pills-sent"] = "";
            Session["pills-create-show"] = "active show";
            Session["pills-confirm-show"] = "";
            Session["pills-sent-show"] = "";
            var customerOrders = db.CustomerOrders.Include(c => c.AspNetUser);
            return View(customerOrders.ToList());
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult OrrderList(DateTime? date_start, DateTime? date_end)
        {

            if (date_start == null)
            {
                date_start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_end == null)
            {
                date_end = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var OrrderList = db.CustomerOrders.Where(s => s.DateCreated >= date_start && s.DateCreated <= date_end
                                                    || s.DateCreated.Value.Day == date_start.Value.Day
                                                    && s.DateCreated.Value.Month == date_start.Value.Month
                                                    && s.DateCreated.Value.Year == date_start.Value.Year
                                                    || s.DateCreated.Value.Day == date_end.Value.Day
                                                    && s.DateCreated.Value.Month == date_end.Value.Month
                                                    && s.DateCreated.Value.Year == date_end.Value.Year);
            return PartialView(OrrderList.ToList());
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult OrrderDetailsList(string order_id)
        {
            CustomerOrder order = db.CustomerOrders.Find(order_id);
            ViewBag.Order = order;
            var OrrderDetailsList = db.OrderDetails.Where(o => o.OrderID == order_id);
            return PartialView(OrrderDetailsList.ToList());
        }

        public ActionResult GetOrderData()
        {
            var userId = User.Identity.GetUserId();
            var query_order = db.CustomerOrders.Where(o => o.AccCustomerID == userId).OrderByDescending(o => o.DateCreated).ToList();
            return PartialView("_Order", query_order);
        }

        public ActionResult GetOrderDetails(string order_id)
        {
            var order = db.CustomerOrders.Find(order_id);
            var orrderDetailsList = db.OrderDetails.Where(o => o.OrderID == order_id).ToList();
            return PartialView("_OrderDetails", new CustomerOrderViewModels
            {
                CustomerOrder = order,
                OrderDetails = orrderDetailsList
            });
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult EditStatus_Order(CustomerOrder order)
        {
            CustomerOrder customerOrder = db.CustomerOrders.Find(order.ID);
            var user = UserManager.FindById(customerOrder.AccCustomerID);
            if (customerOrder.Status == Constants.WAIT_FOR_CONFIRMATION)
            {
                Session["pills-create"] = "active";
                Session["pills-confirm"] = "";
                Session["pills-sent"] = "";

                Session["pills-create-show"] = "active show";
                Session["pills-confirm-show"] = "";
                Session["pills-sent-show"] = "";
            }
            else if (customerOrder.Status == Constants.APPROVED)
            {
                Session["pills-create"] = "";
                Session["pills-confirm"] = "active";
                Session["pills-sent"] = "";

                Session["pills-create-show"] = "";
                Session["pills-confirm-show"] = "active show";
                Session["pills-sent-show"] = "";
            }
            else if (customerOrder.Status == Constants.DELIVERING)
            {
                Session["pills-create"] = "";
                Session["pills-confirm"] = "";
                Session["pills-sent"] = "active";

                Session["pills-create-show"] = "";
                Session["pills-confirm-show"] = "";
                Session["pills-sent-show"] = "active show";
            }
            customerOrder.Status = order.Status;
            if (order.Status == Constants.PAY_IN_ADVANCE)
            {
                customerOrder.PaidAdvance = order.PaidAdvance;
                customerOrder.Note = order.Note;
                customerOrder.PaymentMethod = Constants.BANK_METHOD;
            }
            db.Entry(customerOrder).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { userId = user.Id, orderId = customerOrder.ID, status = customerOrder.Status }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Staff")]
        public async Task EmailStatus(string userId, string orderId, string status)
        {
            status = Constants.StateOder.Single(x => x.Key == int.Parse(status)).Value;
            await UserManager.SendEmailAsync(userId, "Your order " + orderId + " has been updated", "<!DOCTYPE html><html lang='en' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head><title></title><meta content='text/html; charset=utf-8' http-equiv='Content-Type'/><meta content='width=device-width, initial-scale=1.0' name='viewport'/><link href='https://fonts.googleapis.com/css?family=Montserrat' rel='stylesheet' type='text/css'/><style>*{box-sizing: border-box;}body{margin: 0;padding: 0;}a[x-apple-data-detectors]{color: inherit !important;text-decoration: inherit !important;}#MessageViewBody a{color: inherit;text-decoration: none;}p{line-height: inherit}.desktop_hide,.desktop_hide table{mso-hide: all;display: none;max-height: 0px;overflow: hidden;}@media (max-width:700px){.desktop_hide table.icons-inner{display: inline-block !important;}.icons-inner{text-align: center;}.icons-inner td{margin: 0 auto;}.row-content{width: 100% !important;}.mobile_hide{display: none;}.stack .column{width: 100%;display: block;}.mobile_hide{min-height: 0;max-height: 0;max-width: 0;overflow: hidden;font-size: 0px;}.desktop_hide,.desktop_hide table{display: table !important;max-height: none !important;}}</style></head><body style='background-color: #ffffff; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'><table border='0' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #abd373;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='image_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='width:100%;padding-right:0px;padding-left:0px;padding-top:45px;'><div align='center' class='alignment' style='line-height:10px'><a href='#' style='outline:none' tabindex='-1' target='_blank'><img alt='Order Processing' src='https://img.icons8.com/color/480/null/smiling-face-with-heart.png' style='display: block; height: auto; border: 0; width: 306px; max-width: 100%;' title='Cool Burger Walking' width='306'/></a></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-4' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:30px;'><div style='font-family: sans-serif'><div class='' style='font-size: 14px; mso-line-height-alt: 16.8px; color: #33563c; line-height: 1.2; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><strong><span style='font-size:38px;'>Your order's status is now " + status + "</span></strong></p></div></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-5' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:20px;padding-left:60px;padding-right:60px;padding-top:10px;'><div style='font-family: sans-serif'><div class='' style='font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 21.6px; color: #33563c; line-height: 1.8;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 32.4px;'><span style='font-size:18px;'>To track order, click below button for more details</span></p></div></div></td></tr></table><table border='0' cellpadding='10' cellspacing='0' class='button_block block-6' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad'><div align='center' class='alignment'><a href='" + Url.Action("Index", "Manage", null, protocol: Request.Url.Scheme) + "' style='text-decoration:none;display:inline-block;color:#33563c;background-color:#ffffff;border-radius:4px;width:auto;border-top:0px solid #8a3b8f;font-weight:400;border-right:0px solid #8a3b8f;border-bottom:0px solid #8a3b8f;border-left:0px solid #8a3b8f;padding-top:5px;padding-bottom:5px;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;font-size:16px;text-align:center;mso-border-alt:none;word-break:keep-all;' target='_blank'><span style='padding-left:40px;padding-right:40px;font-size:16px;display:inline-block;letter-spacing:normal;'><span dir='ltr' style='word-break: break-word; line-height: 32px;'><strong>View orders</strong></span></span></a></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-7' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-top:10px;padding-right:10px;padding-bottom:55px;padding-left:10px;'><div style='color:#000000;font-size:14px;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;font-weight:400;line-height:120%;text-align:center;direction:ltr;letter-spacing:0px;mso-line-height-alt:16.8px;'><p style='margin: 0;'>Cheers, <br/>Bonsai Garden Team</p></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='icons_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;'><table cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='alignment' style='vertical-align: middle; text-align: center;'><table cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;'></table></td></tr></table></td></tr></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public JsonResult FindOrder(string order_id)
        {
            var emp = new CustomerOrder
            {
                ID = order_id

            };
            return Json(emp);
        }

        [Authorize(Roles = "Admin, Staff")]
        public ActionResult DeleteOrder(CustomerOrder order)
        {
            CustomerOrder customerOrder = db.CustomerOrders.Find(order.ID);

            if (customerOrder.Status == Constants.WAIT_FOR_CONFIRMATION)
            {

                Session["pills-create"] = "active";
                Session["pills-confirm"] = "";
                Session["pills-sent"] = "";

                Session["pills-create-show"] = "active show";
                Session["pills-confirm-show"] = "";
                Session["pills-sent-show"] = "";
            }
            else if (customerOrder.Status == Constants.APPROVED)
            {

                Session["pills-create"] = "";
                Session["pills-confirm"] = "active";
                Session["pills-sent"] = "";

                Session["pills-create-show"] = "";
                Session["pills-confirm-show"] = "active show";
                Session["pills-sent-show"] = "";
            }
            else if (customerOrder.Status == Constants.DELIVERING)
            {

                Session["pills-create"] = "";
                Session["pills-confirm"] = "";
                Session["pills-sent"] = "active";

                Session["pills-create-show"] = "";
                Session["pills-confirm-show"] = "";
                Session["pills-sent-show"] = "active show";
            }
            customerOrder.Status = Constants.PAY_IN_ADVANCE;
            customerOrder.Reason = order.Reason;

            db.Entry(customerOrder).State = EntityState.Modified;
            db.SaveChanges();
            return Json("DeleteOrder", JsonRequestBehavior.AllowGet);
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
