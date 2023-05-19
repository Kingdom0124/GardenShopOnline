using GardenShopOnline.Models;
using System.Linq;
using System.Web.Mvc;

namespace GardenShopOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly BonsaiGardenEntities db = new BonsaiGardenEntities();

        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModels
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }

        //
        // GET: /ShoppingCart/GetMiniCart
        public ActionResult GetMiniCart()
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModels
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return PartialView("_MiniCart", viewModel);
        }

        //
        // GET: /ShoppingCart/GetCheckoutCart
        public ActionResult GetCheckoutCart()
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModels
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return PartialView("_Checkout", viewModel);
        }

        //
        // GET: /Store/AddToCart/5
        public ActionResult AddToCart(int id, int? quantity)
        {
            // Retrieve the product from the database
            var addedproduct = db.Products
                .Single(product => product.ID == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(HttpContext);
            int itemCount = cart.GetItemCount(id);
            // Check if number exceeds product quantity
            if ((itemCount + quantity.GetValueOrDefault(1)) > addedproduct.Quantity)
            {
                return Json(new { error = true, quantity = addedproduct.Quantity }, JsonRequestBehavior.AllowGet);
            }
            cart.AddToCart(addedproduct, quantity);

            // Get item subtotal
            var itemTotal = cart.GetItemTotal(addedproduct.ID);

            var results = new ShoppingCartRemoveViewModels
            {
                ItemTotal = itemTotal,
                CartTotal = cart.GetTotal()
            };
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        //
        // AJAX: /ShoppingCart/DecreaseQuantity/5
        [HttpPost]
        public ActionResult DecreaseQuantity(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(HttpContext);

            // Get the name of the product to display confirmation
            var product = db.Carts
                .Single(item => item.RecordID == id).Product;

            // Remove from cart
            int itemCount = cart.DecreaseQuantity(id);
            var itemTotal = cart.GetItemTotal(product.ID);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModels
            {
                Message = product.Name + " has been removed from cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                ItemTotal = itemTotal,
                DecreaseId = id
            };
            return Json(results);
        }

        //
        // AJAX: /ShoppingCart/RemoveProduct/5
        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(HttpContext);

            // Get the name of the product to display confirmation
            var product = db.Carts
                .Single(item => item.RecordID == id).Product;

            // Remove from cart
            int itemCount = cart.RemoveProduct(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModels
            {
                Message = product.Name + " has been removed from cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        //
        // GET: /ShoppingCart/CartSummary
        public int CartSummary()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            return cart.GetCount();
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