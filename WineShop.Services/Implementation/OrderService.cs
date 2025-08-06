using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Repository;
using WineShop.Repository.Implementation;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;

namespace WineShop.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, ApplicationDbContext context, IProductRepository productRepository)
        {
            this._orderRepository = orderRepository;
            this._context = context;
            this._productRepository = productRepository;
        }
        public List<Order> GetAllOrders()
        {
            return this._orderRepository.GetAllOrders();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return this._orderRepository.GetOrderDetaills(model);
        }

        public void PlaceOrder(string userId)
        {
            // 1. Најди ја кошничката за корисникот
            var cart = _context.ShoppingCarts
                .Include(sc => sc.ProductInShoppingCarts)
                .ThenInclude(p => p.Product)
                .FirstOrDefault(sc => sc.OwnerId == userId);

            if (cart == null || !cart.ProductInShoppingCarts.Any())
                throw new Exception("Кошничката е празна или не постои.");

            // 2. Подготви листа од нарачани производи
            var productInOrders = new List<ProductInOrder>();
            var orderId = Guid.NewGuid(); // Генерирај ID однапред

            foreach (var item in cart.ProductInShoppingCarts)
            {
                // 3. Намалување на залиха
                _productRepository.ReduceProductQuantity(item.ProductId, item.Quantity);

                // 4. Додај во листата
                productInOrders.Add(new ProductInOrder
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    OrderId = orderId,
                    Quantity = item.Quantity
                });
            }

            // 5. Креирај нарачка
            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                ProductInOrders = productInOrders
            };

            // 6. Сними нарачката
            _context.Orders.Add(order);

            // 7. Празнење на кошничката
            _context.ProductInShoppingCarts.RemoveRange(cart.ProductInShoppingCarts);

            // 8. Зачувување на сите промени
            _context.SaveChanges();
           
        }


    }
}
