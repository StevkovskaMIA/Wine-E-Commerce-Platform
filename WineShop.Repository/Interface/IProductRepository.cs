using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineShop.Repository.Interface
{
    public interface IProductRepository
    {
        void ReduceProductQuantity(Guid productId, int quantity);

    }
}
