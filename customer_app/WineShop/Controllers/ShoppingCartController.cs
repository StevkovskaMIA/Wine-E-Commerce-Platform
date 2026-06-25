using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Climate;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;
using WineShop.Domain.Payment;
using WineShop.Repository;
using WineShop.Repository.Implementation;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;



namespace WineShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserRepository _userRepository;
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;




        public ShoppingCartController(IOrderService orderService, IShoppingCartService shoppingCartService, IUserRepository userRepository, IOptions<StripeSettings> stripeSettings, IEmailService emailService)
        {
            _shoppingCartService = shoppingCartService;
            _userRepository = userRepository;
            _stripeSettings = stripeSettings.Value;
            _orderService = orderService;
            _emailService = emailService;
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
                return Json(new { success = false, message = "Невалидно барање." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var cart = _shoppingCartService.getShoppingCartInfo(userId);
            if (cart == null || cart.Products == null || !cart.Products.Any())
                return Json(new { success = false, message = "Кошничката е празна." });

            if (string.IsNullOrWhiteSpace(request.Phone))
                return Json(new { success = false, message = "Телефонот е задолжителен." });

            var phoneRegex = new Regex(@"^(\+389\s?7\d\s?\d{3}\s?\d{3}|07\d\s?\d{3}\s?\d{3})$");

            if (!phoneRegex.IsMatch(request.Phone))
                return Json(new { success = false, message = "Внеси валиден телефон: +389 7X XXX XXX или 07X XXX XXX." });

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

        /*public IActionResult SuccessPayment()
        {
                 
             return View();
        }*/

        public async Task<IActionResult> SuccessPayment()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var user = _userRepository.Get(userId);
            var order = _orderService.GetLatestOrderByUserId(userId);

            if (user != null && order != null)
            {
                string address = user.Address ?? "";

                string formattedAddress = string.IsNullOrWhiteSpace(address)
                    ? ""
                    : address
                        .Replace("\r\n", "<br/>")
                        .Replace("\n", "<br/>")
                        .Replace("\r", "<br/>");

                string addressSection = !string.IsNullOrWhiteSpace(formattedAddress)
                    ? $@"
            <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;' />

            <p style='font-size: 14px; color: #666; margin-bottom: 5px;'><b>Ship to:</b></p>

            <p style='font-size: 15px; color: #333; line-height: 1.6;'>
                {formattedAddress}
            </p>"
                    : "";

                var userMail = new EmailMessage
                {
                    MailTo = user.Email,
                    Subject = "Успешна нарачка - Винарија Кувин",
                    Content = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e5e5e5; border-radius: 10px;'>
                <h2 style='color: #7b1e3a; margin-bottom: 20px;'>Ви благодариме за нарачката!</h2>

                <p style='font-size: 15px; color: #333;'><b>Број на нарачка:</b> {order.Id}</p>

                <p style='font-size: 15px; color: #333;'>
                    Нарачката ќе биде доставена во рок од <b>1-2 работни дена</b>.
                </p>

                {addressSection}

                <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;' />

                <p style='font-size: 13px; color: #888;'>
                    Ви благодариме што купувате од Винарија Кувин.
                </p>
            </div>",
                    status = true
                };

                var adminMail = new EmailMessage
                {
                    MailTo = "mia.stevkovska@yahoo.com",
                    Subject = "Нова успешна нарачка",
                    Content = $@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e5e5e5; border-radius: 10px;'>
        <h2 style='color: #7b1e3a; margin-bottom: 20px;'>Нова успешно платена нарачка</h2>

        <p style='font-size: 15px; color: #333;'><b>Број на нарачка:</b> {order?.Id}</p>
        <p style='font-size: 15px; color: #333;'><b>Корисник:</b> {order?.User?.UserName}</p>
        <p style='font-size: 15px; color: #333;'><b>Емаил:</b> {order?.User?.Email}</p>
        <p style='font-size: 15px; color: #333;'><b>Начин на достава:</b> {(order?.DeliveryType == "delivery" ? "Достава" : "Лично превземање")}</p>
        <p style='font-size: 15px; color: #333;'><b>Телефон:</b> {order?.DeliveryPhone}</p>
        <p style='font-size: 15px; color: #333;'><b>Град:</b> {order?.DeliveryCity}</p>
        <p style='font-size: 15px; color: #333;'><b>Адреса:</b> {order?.DeliveryAddress}</p>

        <p style='font-size: 14px; color: #666; margin-top: 20px;'>
            Провери ја нарачката во админ панелот за повеќе детали.
        </p>
    </div>",
                    status = true
                };

                await _emailService.SendEmailAsync(userMail);
                await _emailService.SendEmailAsync(adminMail);
            }

            return View();
        }


        public IActionResult NotSuccessPayment()
        {
            return View();
        }


  



    }
}
