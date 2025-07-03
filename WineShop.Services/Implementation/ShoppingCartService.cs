using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;

namespace WineShop.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {

        private readonly IRepository<ShoppingCart> _shoppingCartRepositorty;
        private readonly IRepository<Order> _orderRepositorty;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IRepository<ProductInOrder> _productInOrderRepositorty;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<EmailMessage> mailRepository,IRepository<ShoppingCart> shoppingCartRepository, IRepository<ProductInOrder> productInOrderRepositorty, IRepository<Order> orderRepositorty, IUserRepository userRepository)
        {
            _shoppingCartRepositorty = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepositorty = orderRepositorty;
            _productInOrderRepositorty = productInOrderRepositorty;
            _mailRepository = mailRepository;
        }
        public bool deleteProductFromShoppingCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();

                userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepositorty.Update(userShoppingCart);

                return true;
            }

            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            //od najaveniot korisnik ja zimame shopping-kartickata
            var userShoppingCart = loggedInUser.UserCart;

            //od najaveniot korisnik gi zimame produktitite koi se vo shopping-kartickata
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
            return shoppingCartDtoItem;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                //id na najaven korisnik so negovata kartichka
                var loggedInUser = this._userRepository.Get(userId);

                //kartichkata so svoite produkti
                var userShoppingCart = loggedInUser.UserCart;




                EmailMessage message = new EmailMessage();
                message.MailTo = loggedInUser.Email;
                message.Subject = "Successfully created order";
                message.status = false;


                //soodvetnata karticka i kreiram order
                Order orderItem = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser
                };

                //orderot se dodava vo DB
                this._orderRepositorty.Insert(orderItem);

                //prazna lista so ProductInOrder
                List<ProductInOrder> productInOrders = new List<ProductInOrder>();

                //tuka ja popolnuvam listata ProductInOrder
                var result = userShoppingCart.ProductInShoppingCarts
                    .Select(z => new ProductInOrder
                    {
                        OrderId = orderItem.Id, //eden produkt koj ke e vo OderId onoj sto go kreiravme
                        ProductId = z.Product.Id, //id na produktot
                        OrderedProduct = z.Product, //onoj produkt koj so go selektirame example: Stanushina
                        UserOrder = orderItem,
                        Quantity = z.Quantity
                    }).ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your order is completed. The order contains: ");

                var totalPrice = 0.0;
                for (int i = 1; i<= result.Count; i++)
                {
                    var item = result[i-1];
                    sb.AppendLine(i.ToString() + " " + item.OrderedProduct.ProductName + " with price of: " + item.OrderedProduct.Price +  " and quantity of: " + item.Quantity);
                    totalPrice += item.Quantity * item.OrderedProduct.Price;
                }

                sb.AppendLine("Total Price: " +totalPrice.ToString() +"мкд.");
                message.Content = sb.ToString();
                //gi vnesuvam vo ramkite na DB
                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                   this._productInOrderRepositorty.Insert(item);
                }

                //ja cistam kartickata
                loggedInUser.UserCart.ProductInShoppingCarts.Clear();
                this._mailRepository.Insert(message);

                this._userRepository.Update(loggedInUser);

                return true;
            }   
            return false;

            
        }
    }
}
