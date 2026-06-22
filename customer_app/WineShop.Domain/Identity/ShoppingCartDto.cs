using WineShop.Domain.DomainModels;

namespace WineShop.Domain.Identity
{
    public class ShoppingCartDto
    {
        public List<ProductInShoppingCart> Products { get; set; }
        public double TotalPrice { get; set; }
    }
}
