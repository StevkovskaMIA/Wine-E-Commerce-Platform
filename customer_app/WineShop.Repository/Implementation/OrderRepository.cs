using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Repository.Interface;

namespace WineShop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.ProductInOrders)
                .Include(z => z.User)
                .Include("ProductInOrders.OrderedProduct")

                .ToListAsync().Result;
        }

        public Order GetOrderDetaills(BaseEntity model)
        {
            return entities
                .Include(z => z.ProductInOrders)
                .Include(z => z.User)
                .Include("ProductInOrders.OrderedProduct")
                .SingleOrDefaultAsync(z => z.Id == model.Id).Result;

        }
    }
}
