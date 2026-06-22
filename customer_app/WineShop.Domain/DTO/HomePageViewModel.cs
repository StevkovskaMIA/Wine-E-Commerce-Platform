using WineShop.Domain.DomainModels;

namespace WineShop.Domain.DTO
{
    public class HomePageViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<TastingPackage> TastingPackages { get; set; } = new();
    }
}