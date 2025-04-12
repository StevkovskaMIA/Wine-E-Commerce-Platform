using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using WineShop.Repository;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;

namespace WineShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserRepository _userRepository;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IUserRepository userRepository)
        {
            _shoppingCartService = shoppingCartService;
            _userRepository = userRepository;
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

        public IActionResult Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");

            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
    
            }
        }
    }
}
