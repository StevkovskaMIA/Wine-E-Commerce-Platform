using WineShop.Domain.DomainModels;
using WineShop.Repository.Interface;
using WineShop.Service.Interface;

namespace WineShop.Service.Implementation
{
    public class TastingPackageService : ITastingPackageService
    {
        private readonly ITastingPackageRepository _tastingPackageRepository;

        public TastingPackageService(ITastingPackageRepository tastingPackageRepository)
        {
            _tastingPackageRepository = tastingPackageRepository;
        }

        public List<TastingPackage> GetAllTastingPackages()
        {
            return _tastingPackageRepository.GetAll().ToList();
        }
    }
}