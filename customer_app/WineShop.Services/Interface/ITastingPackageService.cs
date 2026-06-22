using WineShop.Domain.DomainModels;

namespace WineShop.Service.Interface
{
    public interface ITastingPackageService
    {
        List<TastingPackage> GetAllTastingPackages();
    }
}