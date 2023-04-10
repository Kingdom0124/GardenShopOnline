using GardenShopOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = GardenShopOnline.Helpers.Constants;


namespace GardenShopOnline.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();
        public CheckoutController()
        {
        }

        public CheckoutController(ApplicationUserManager userManager)
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
        public ActionResult AddressAndPayment()
        {
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            var customerOrder = new CustomerOrder()
            {
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.PhoneNumber
            };
            ViewBag.BankPayments = db.BankPayments.Where(b => b.Status == Constants.SHOW_STATUS).ToList();
            return View(customerOrder);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> AddressAndPayment(CustomerOrder order, string payment)
        {
            TryUpdateModel(order);
            try
            {
                var cart = ShoppingCart.GetCart(HttpContext);
                // Check product quantity and order quantity
                string check = cart.CheckOrder();
                if (check != string.Empty)
                {
                    // Display error when order quantity exceeds product quantity
                    var userId = User.Identity.GetUserId();
                    var user = db.AspNetUsers.Find(userId);
                    ViewBag.BankPayments = db.BankPayments.Where(b => b.Status == Constants.SHOW_STATUS).ToList();
                    ViewBag.Error = check;
                    return View(order);
                }
                else
                {
                    order.ID = "#" + DateTime.Now.ToString("yyMMddHHmmssff");
                    order.AccCustomerID = User.Identity.GetUserId();
                    order.DateCreated = DateTime.Now;
                    if (payment == "bank")
                    {
                        order.Status = Constants.PAY_IN_ADVANCE;
                        order.PaymentMethod = Constants.BANK_METHOD;
                        order.PaidAdvance = order.Total;
                    }
                    else
                    {
                        order.Status = Constants.WAIT_FOR_CONFIRMATION;
                        order.PaymentMethod = Constants.CASH_METHOD;
                    }

                    // Save Order
                    db.CustomerOrders.Add(order);
                    db.SaveChanges();
                    // Process the order
                    string quality_product = cart.CreateOrder(order);
                    if (quality_product != string.Empty)
                    {
                        var mail_addmin = db.AspNetUsers.Where(a => a.AspNetRoles.Any(r => r.Name == "Admin")).ToList();
                        foreach (var item in mail_addmin)
                        {
                            await UserManager.SendEmailAsync(item.Id, "Product inventory is low", "<!DOCTYPE html><html lang='en' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head><title></title><meta content='text/html; charset=utf-8' http-equiv='Content-Type'/><meta content='width=device-width, initial-scale=1.0' name='viewport'/><link href='https://fonts.googleapis.com/css?family=Montserrat' rel='stylesheet' type='text/css'/><style>*{box-sizing: border-box;}body{margin: 0;padding: 0;}a[x-apple-data-detectors]{color: inherit !important;text-decoration: inherit !important;}#MessageViewBody a{color: inherit;text-decoration: none;}p{line-height: inherit}.desktop_hide,.desktop_hide table{mso-hide: all;display: none;max-height: 0px;overflow: hidden;}@media (max-width:700px){.desktop_hide table.icons-inner{display: inline-block !important;}.icons-inner{text-align: center;}.icons-inner td{margin: 0 auto;}.row-content{width: 100% !important;}.mobile_hide{display: none;}.stack .column{width: 100%;display: block;}.mobile_hide{min-height: 0;max-height: 0;max-width: 0;overflow: hidden;font-size: 0px;}.desktop_hide,.desktop_hide table{display: table !important;max-height: none !important;}}</style></head><body style='background-color: #ffffff; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'><table border='0' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #abd373;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='image_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='width:100%;padding-right:0px;padding-left:0px;padding-top:45px;'><div align='center' class='alignment' style='line-height:10px'><a href='#' style='outline:none' tabindex='-1' target='_blank'><img alt='Order Processing' src='https://img.icons8.com/cute-clipart/512/null/commercial.png' style='display: block; height: auto; border: 0; width: 306px; max-width: 100%;' title='Cool Burger Walking' width='306'/></a></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-4' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:30px;'><div style='font-family: sans-serif'><div class='' style='font-size: 14px; mso-line-height-alt: 16.8px; color: #33563c; line-height: 1.2; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><strong><span style='font-size:38px;'> " + quality_product + "</span></strong></p></div></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-5' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:20px;padding-left:60px;padding-right:60px;padding-top:10px;'><div style='font-family: sans-serif'><div class='' style='font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 21.6px; color: #33563c; line-height: 1.8;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 32.4px;'><span style='font-size:18px;'></span></p></div></div></td></tr></table><table border='0' cellpadding='10' cellspacing='0' class='button_block block-6' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad'><div align='center' class='alignment'></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-7' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-top:10px;padding-right:10px;padding-bottom:55px;padding-left:10px;'><div style='color:#000000;font-size:14px;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;font-weight:400;line-height:120%;text-align:center;direction:ltr;letter-spacing:0px;mso-line-height-alt:16.8px;'><p style='margin: 0;'>Cheers, <br/>Bonsai Garden Team</p></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='icons_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;'><table cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='alignment' style='vertical-align: middle; text-align: center;'><table cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;'></table></td></tr></table></td></tr></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>");
                        }
                    }
                    if (order.Total >= 10000000)
                    {
                        var mail_addmin = db.AspNetUsers.Where(a => a.AspNetRoles.Any(r => r.Name == "Admin")).ToList();
                        foreach (var item in mail_addmin)
                        {
                            await UserManager.SendEmailAsync(item.Id, "Order " + order.ID + " has a large cost", "<!DOCTYPE html><html lang='en' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head><title></title><meta content='text/html; charset=utf-8' http-equiv='Content-Type'/><meta content='width=device-width, initial-scale=1.0' name='viewport'/><link href='https://fonts.googleapis.com/css?family=Montserrat' rel='stylesheet' type='text/css'/><style>*{box-sizing: border-box;}body{margin: 0;padding: 0;}a[x-apple-data-detectors]{color: inherit !important;text-decoration: inherit !important;}#MessageViewBody a{color: inherit;text-decoration: none;}p{line-height: inherit}.desktop_hide,.desktop_hide table{mso-hide: all;display: none;max-height: 0px;overflow: hidden;}@media (max-width:700px){.desktop_hide table.icons-inner{display: inline-block !important;}.icons-inner{text-align: center;}.icons-inner td{margin: 0 auto;}.row-content{width: 100% !important;}.mobile_hide{display: none;}.stack .column{width: 100%;display: block;}.mobile_hide{min-height: 0;max-height: 0;max-width: 0;overflow: hidden;font-size: 0px;}.desktop_hide,.desktop_hide table{display: table !important;max-height: none !important;}}</style></head><body style='background-color: #ffffff; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'><table border='0' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #abd373;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='image_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='width:100%;padding-right:0px;padding-left:0px;padding-top:45px;'><div align='center' class='alignment' style='line-height:10px'><a href='#' style='outline:none' tabindex='-1' target='_blank'><img alt='Order Processing' src='https://img.icons8.com/bubbles/480/null/money-bag.png' style='display: block; height: auto; border: 0; width: 306px; max-width: 100%;' title='Cool Burger Walking' width='306'/></a></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-4' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:30px;'><div style='font-family: sans-serif'><div class='' style='font-size: 14px; mso-line-height-alt: 16.8px; color: #33563c; line-height: 1.2; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><strong><span style='font-size:38px;'> Orders with large costs: " + order.Total.ToString("N0") + "đ Please confirm with the customer.</span></strong></p></div></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='text_block block-5' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-bottom:20px;padding-left:60px;padding-right:60px;padding-top:10px;'><div style='font-family: sans-serif'><div class='' style='font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 21.6px; color: #33563c; line-height: 1.8;'><p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 32.4px;'><span style='font-size:18px;'></span></p></div></div></td></tr></table><table border='0' cellpadding='10' cellspacing='0' class='button_block block-6' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad'><div align='center' class='alignment'></div></td></tr></table><table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-7' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'><tr><td class='pad' style='padding-top:10px;padding-right:10px;padding-bottom:55px;padding-left:10px;'><div style='color:#000000;font-size:14px;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;font-weight:400;line-height:120%;text-align:center;direction:ltr;letter-spacing:0px;mso-line-height-alt:16.8px;'><p style='margin: 0;'>Cheers, <br/>Bonsai Garden Team</p></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tbody><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 680px;' width='680'><tbody><tr><td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'><table border='0' cellpadding='0' cellspacing='0' class='icons_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='pad' style='vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;'><table cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'><tr><td class='alignment' style='vertical-align: middle; text-align: center;'><table cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;'></table></td></tr></table></td></tr></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>");
                        }
                    }
                    return RedirectToAction("Complete", new { id = order.ID });
                }
            }
            catch
            {
                // Invalid - redisplay with errors
                return View(order);
            }
        }

        [HttpGet]
        public ActionResult Complete(string id)
        {
            var userId = User.Identity.GetUserId();
            // Validate customer owns this order
            bool isValid = db.CustomerOrders.Any(
                o => o.ID == id &&
                o.AccCustomerID == userId);

            if (isValid)
            {
                ViewData["OrderId"] = id;
                return View();
            }
            else
            {
                return View("Error");
            }
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