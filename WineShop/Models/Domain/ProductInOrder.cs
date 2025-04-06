using System.ComponentModel.DataAnnotations;

namespace WineShop.Models.Domain
{
    public class ProductInOrder
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product OrderedProduct { get; set; }

        public Guid OrderId { get; set; }
        public Order UserOrder { get; set; }

        public int Quantity { get; set; }
    }
}
