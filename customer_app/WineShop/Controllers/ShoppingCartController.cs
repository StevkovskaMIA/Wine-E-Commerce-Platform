using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Climate;
using System.Security.Claims;
using WineShop.Domain.Payment;
using WineShop.Repository;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;
using WineShop.Domain.DTO;


namespace WineShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserRepository _userRepository;
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderService _orderService;




        public ShoppingCartController(IOrderService orderService, IShoppingCartService shoppingCartService, IUserRepository userRepository, IOptions<StripeSettings> stripeSettings)
        {
            _shoppingCartService = shoppingCartService;
            _userRepository = userRepository;
            _stripeSettings = stripeSettings.Value;
            _orderService = orderService;


        }
        public IActionResult Index()
        {

            //go naogja userot
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._shoppingCartService.getShoppingCartInfo(userId));

        }

        public IActionResult DeleteFromShoppingCart(Guid id)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.deleteProductFromShoppingCart(userId, id);

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

        }

        //public IActionResult Order()
        //{
        //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var result = this._shoppingCartService.orderNow(userId);

        //    if (result)
        //    {
        //        return RedirectToAction("Index", "ShoppingCart");

        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "ShoppingCart");

        //    }
        //}

        // da ne kreira naracka ako kosnickata e prazna
        public IActionResult Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = _shoppingCartService.getShoppingCartInfo(userId);
            if (cart == null || cart.Products == null || !cart.Products.Any())
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var result = this._shoppingCartService.orderNow(userId);

            return RedirectToAction("Index", "ShoppingCart");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrderWithDelivery([FromBody] OrderRequestDto request)
        {
            if (request == null)
                return Json(new { success = false, message = "Невалиден барање." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var cart = _shoppingCartService.getShoppingCartInfo(userId);
            if (cart == null || cart.Products == null || !cart.Products.Any())
                return Json(new { success = false, message = "Кошничката е празна." });

            if (string.IsNullOrWhiteSpace(request.Phone))
                return Json(new { success = false, message = "Телефонот е задолжителен." });

            if (request.DeliveryType == "delivery")
            {
                if (string.IsNullOrWhiteSpace(request.Address))
                    return Json(new { success = false, message = "Адресата е задолжителна." });

                if (string.IsNullOrWhiteSpace(request.City))
                    return Json(new { success = false, message = "Градот е задолжителен за достава." });
            }

            _orderService.PlaceOrder(
                userId,
                request.DeliveryType,
                request.Address,
                request.City,
                request.Phone);

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult PayOrder()
        {
            Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var cart = _shoppingCartService.getShoppingCartInfo(userId);

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
        {
            new Stripe.Checkout.SessionLineItemOptions
            {
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    Currency = "mkd",
                    UnitAmount = Convert.ToInt32(cart.TotalPrice) * 100, 
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "WineShop Order"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = Url.Action("SuccessPayment", "ShoppingCart", null, Request.Scheme),
                CancelUrl = Url.Action("NotSuccessPayment", "ShoppingCart", null, Request.Scheme)
            };

            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            return Json(new { id = session.Id });
        }
        
        public IActionResult SuccessPayment()
        {
                 
             return View();
        }

        

        public IActionResult NotSuccessPayment()
        {
            return View();
        }


  



    }
}
