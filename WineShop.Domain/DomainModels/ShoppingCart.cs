using System.ComponentModel.DataAnnotations;
using WineShop.Domain.Identity;

namespace WineShop.Domain.DomainModels
{
    public class ShoppingCart :BaseEntity
    {

        public string? OwnerId { get; set; }

        public EShopApplicationUser? Owner { get; set; }

        public virtual ICollection<ProductInShoppingCart>? ProductInShoppingCarts { get; set; }

    }
}
