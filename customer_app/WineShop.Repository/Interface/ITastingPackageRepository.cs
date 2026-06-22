using WineShop.Domain.DomainModels;

namespace WineShop.Repository.Interface
{
    public interface ITastingPackageRepository
    {
        List<TastingPackage> GetAll();
        TastingPackage GetById(Guid id);
    }
}