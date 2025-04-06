using System.ComponentModel.DataAnnotations;
using WineShop.Models.Identity;

namespace WineShop.Models.Domain
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId {  get; set; }
        
        public EShopApplicationUser User { get; set; }

        public virtual ICollection<ProductInOrder> Products { get; set; }

    }
}
