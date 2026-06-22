using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;

namespace WineShop.Repository.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        void ReduceProductQuantity(Guid productId, int quantity);

        //vrakja konkreten proizvod so site nagradi, go koristam vo Details, Edit za da prikazam site nagradi na toj proizvod
        Product GetProductWithAwards(Guid? id);

        //site prozivodi so nagradi, go koristam vo Index da prikazam samo tie proizvodi koi imaat nagradi
        List<Product> GetAllWithAwards();


    }
}
