using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WineShop.Data;
using WineShop.Models.Identity;
using WineShop.Models.DTO;
using WineShop.Models.Domain;

namespace WineShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EShopApplicationUser> _userManager;


        public ShoppingCartController(ApplicationDbContext context, UserManager<EShopApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.
                Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
            var userShoppingCart = loggedInUser.UserCart;

            var productPrice = userShoppingCart.ProductInShoppingCarts.Select(z => new
            {
                ProductPrice = z.Product.Price,
                Quantity = z.Quantity
            }).ToList();

            var totalPrice = 0;
            foreach (var item in productPrice)
            {
                totalPrice += item.ProductPrice * item.Quantity;
            }

            // var allProducts = UserShoppingCart.ProductInShoppingCarts.Select(z => z.Product).ToList();

            ShoppingCartDto shoppingCartDtoItem = new ShoppingCartDto
            {
                Products = userShoppingCart.ProductInShoppingCarts.ToList(),
                TotalPrice = totalPrice
            };
            
            return View(shoppingCartDtoItem);
    
        }

        public async Task<IActionResult> DeleteFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.
                Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedInUser.UserCart;
            
            userShoppingCart.ProductInShoppingCarts.
                Remove(userShoppingCart.ProductInShoppingCarts
                .Where(z => z.ProductId == id).FirstOrDefault());

            _context.Update(userShoppingCart);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "ShoppingCart");

        }

        public async Task<IActionResult> Order()
        {
            //id na najaven korisnik so negovata kartichka
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.
                Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

            //kartichkata so svoite produkti
            var userShoppingCart = loggedInUser.UserCart;

            //soodvetnata karticka i kreiram order
            Order orderItem = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };
            
            //orderot se dodava vo DB
            _context.Add(orderItem);


            //prazna lista so ProductInOrder
            List<ProductInOrder> productInOrders = new List<ProductInOrder>();

            //tuka ja popolnuvam listata ProductInOrder
            productInOrders = userShoppingCart.ProductInShoppingCarts
                .Select(z => new ProductInOrder
                {
                    OrderId = orderItem.Id, //eden produkt koj ke e vo OderId onoj sto go kreiravme
                    ProductId = z.Product.Id, //id na produktot
                    OrderedProduct = z.Product, //onoj produkt koj so go selektirame example: Stanushina
                    UserOrder = orderItem
                }).ToList();

            //gi vnesuvam vo ramkite na DB

            foreach(var item in productInOrders)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
            }

            //ja cistam kartickata
            loggedInUser.UserCart.ProductInShoppingCarts.Clear();

            _context.Update(loggedInUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ShoppingCart");
        } 
    }   
}
