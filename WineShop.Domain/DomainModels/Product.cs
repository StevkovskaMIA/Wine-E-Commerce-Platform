using System.ComponentModel.DataAnnotations;

namespace WineShop.Domain.DomainModels
{
    public class Product : BaseEntity
    {


        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductImage { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public int Quantity { get; set; }


        public virtual ICollection<ProductInShoppingCart>? ProductInShoppingCarts { get; set; }

        [ScaffoldColumn(false)]
        public IEnumerable<ProductInOrder>? Orders { get; set; } = new List<ProductInOrder>();



    }
}
