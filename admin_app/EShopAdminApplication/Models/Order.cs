namespace EShopAdminApplication.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public EShopApplicationUser User { get; set; }

        public ICollection<ProductInOrder> ProductInOrders { get; set; }

        public string DeliveryType { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryCity { get; set; }
        public string? DeliveryPhone { get; set; }
    }
}
