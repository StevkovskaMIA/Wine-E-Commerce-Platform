using System.ComponentModel.DataAnnotations;
using WineShop.Domain.Identity;

namespace WineShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
      
        public string UserId {  get; set; }
        
        public EShopApplicationUser User { get; set; }

        public IEnumerable<ProductInOrder> ProductInOrders { get; set; }

    }
}
