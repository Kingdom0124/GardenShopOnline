using GardenShopOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardenShopOnline.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AspNetUsersController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        public AspNetUsersController()
        {
        }

        public AspNetUsersController(ApplicationUserManager userManager)
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

        // GET: AspNetUsers
        public ActionResult Index()
        {
            ViewData["role_id"] = new SelectList(db.AspNetRoles.ToList(), "id", "name");
            return View();
        }

        public JsonResult GetData()
        {
            // Get user data from database
            return Json(db.AspNetUsers.Where(u => u.AspNetRoles.Any()).Select(u => new
            {
                u.Id,
                u.StaffId,
                u.FullName,
                u.Email,
                u.AspNetRoles.FirstOrDefault().Name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string email, string fullName, string role_id, string password)
        {
            var query_email = UserManager.FindByEmail(email);
            var role = db.AspNetRoles.Find(role_id);
            if (query_email == null)
            {
                string StaffId = "BS-NV-";
                var check_staff = db.AspNetUsers.Where(a => a.StaffId != null).OrderByDescending(s => s.DateCreated).FirstOrDefault();
                if (string.IsNullOrEmpty(check_staff.StaffId) || check_staff.StaffId.Contains(StaffId) == false)
                {
                    StaffId += 1.ToString("000");
                }
                else
                {
                    int number = int.Parse(check_staff.StaffId.Replace(StaffId, "")) + 1;
                    StaffId += number.ToString("000");
                }

                ApplicationUser user = new ApplicationUser()
                {
                    Email = email,
                    UserName = email,
                    StaffId = StaffId,
                    FullName = fullName,
                    DateCreated = DateTime.Now
                };

                IdentityResult result = UserManager.Create(user, password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, role.Name);
                    return Json(new { success = true, message = "Add succeeded!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = true, message = result.Errors }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { error = true, message = "This email is already exists!" }, JsonRequestBehavior.AllowGet);
        }

        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (user.Roles.Count > 0)
            {
                // Set selected value for user role
                ViewData["role_id"] = new SelectList(db.AspNetRoles.ToList(), "id", "name", user.Roles.FirstOrDefault().RoleId);
            }
            else
            {
                // Populate new role select list
                ViewData["role_id"] = new SelectList(db.AspNetRoles.ToList(), "id", "name");
            }
            return PartialView("_Edit", aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, string fullName, string role_id)
        {
            var user = UserManager.FindById(id);
            string oldRole = UserManager.GetRoles(id).FirstOrDefault();
            AspNetRole role = db.AspNetRoles.Find(role_id);

            user.FullName = fullName;
            UserManager.Update(user);

            if (oldRole != null)
            {
                // Update user role
                UserManager.RemoveFromRole(id, oldRole);
                UserManager.AddToRole(id, role.Name);
            }
            else
            {
                // Add user to role
                UserManager.AddToRole(id, role.Name);
            }

            return Json(new { success = true, message = "Update user succeeded!" }, JsonRequestBehavior.AllowGet);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            // Declare variables
            ApplicationUser user = UserManager.FindById(id);

            try
            {
                // Delete user
                UserManager.Delete(user);
            }
            catch
            {
                return Json(new { error = true, message = "Can't delete this user! Please try again later." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Delete succeeded!" }, JsonRequestBehavior.AllowGet);
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
