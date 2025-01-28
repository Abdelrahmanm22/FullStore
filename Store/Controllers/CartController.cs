using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Contexts;
using Store.Models;
using Store.Services;

namespace Store.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly decimal shippingFee;
        private readonly UserManager<User> _userManager;

        public CartController(AppDbContext dbContext,IConfiguration configuration,UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, dbContext);
            decimal subTotal = CartHelper.GetSubtotal(cartItems);


            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subTotal;
            ViewBag.Total = subTotal + shippingFee;


            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutViewModel model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, dbContext);
            decimal subTotal = CartHelper.GetSubtotal(cartItems);


            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subTotal;
            ViewBag.Total = subTotal + shippingFee;

            if (!ModelState.IsValid) {
                return View(model);
            }


            if(cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Your cart items is empty";
                return View(model);
            }

            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request,Response, dbContext);
            decimal total = CartHelper.GetSubtotal(cartItems) + shippingFee;
            int cartSize = 0;
            foreach (var item in cartItems) {
                cartSize += item.Quantity;
            }

            string deliveryAddress = TempData["DeliveryAddress"] as string??"";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();


            if (cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0) {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Total = total;
            ViewBag.CartSize = cartSize;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(int any)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, dbContext);

            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();


            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) {
                return RedirectToAction("Index", "Home");
            }

            var order = new Order
            {
                ClientId = appUser.Id,
                Itmes = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                PaymentDetails ="",
                OrderStatus = "created",
                CreatedAt = DateTime.Now,

            };
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();

            Response.Cookies.Delete("shopping_cart");
            ViewBag.SuccessMessage = "Order created successfully";
            return View();
        }

    }
}
