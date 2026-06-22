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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;
        
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public void ReduceProductQuantity(Guid productId, int quantity)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
                throw new Exception("Производот не е пронајден.");

            if (product.Quantity < quantity)
                throw new Exception("Недоволно залиха.");

            product.Quantity -= quantity;
            context.Products.Update(product);
        }
        public Product GetProductWithAwards(Guid? id)
        {
            return context.Products
                .Include(x => x.ProductAwards)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Product> GetAllWithAwards()
        {
            return context.Products
                .Include(x => x.ProductAwards)
                .ToList();
        }
    }
}
