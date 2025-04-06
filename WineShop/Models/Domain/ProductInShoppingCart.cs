using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WineShop.Models.Domain
{
    public class ProductInShoppingCart
    {
        [Key]

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }


        public Product Product { get; set; }

        public Guid ShoppingCartId { get; set; }


        public ShoppingCart? ShoppingCart { get; set; }

        public int Quantity { get; set; }


    }
}
