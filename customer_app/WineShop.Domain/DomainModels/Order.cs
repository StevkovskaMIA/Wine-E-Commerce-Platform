using System.ComponentModel.DataAnnotations;
using WineShop.Domain.Identity;

namespace WineShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
      
        public string UserId {  get; set; }
        
        public EShopApplicationUser User { get; set; }

        public IEnumerable<ProductInOrder> ProductInOrders { get; set; }

        [Required]
        public string DeliveryType { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryCity { get; set; }
        public string? DeliveryPhone { get; set; }
    }
}
