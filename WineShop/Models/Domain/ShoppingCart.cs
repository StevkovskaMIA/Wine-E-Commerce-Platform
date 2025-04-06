using System.ComponentModel.DataAnnotations;
using WineShop.Models.Identity;

namespace WineShop.Models.Domain
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }

        public string? OwnerId { get; set; }

        public EShopApplicationUser? Owner { get; set; }

        public virtual ICollection<ProductInShoppingCart>? ProductInShoppingCarts { get; set; }

    }
}
