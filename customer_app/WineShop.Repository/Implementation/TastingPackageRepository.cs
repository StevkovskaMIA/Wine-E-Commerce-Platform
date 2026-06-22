using Microsoft.EntityFrameworkCore;
using WineShop.Domain.DomainModels;
using WineShop.Repository.Interface;

namespace WineShop.Repository.Implementation
{
    public class TastingPackageRepository : ITastingPackageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TastingPackage> entities;

        public TastingPackageRepository(ApplicationDbContext context)
        {
            _context = context;
            entities = context.Set<TastingPackage>();
        }

        public List<TastingPackage> GetAll()
        {
            return entities.ToList();
        }

        public TastingPackage GetById(Guid id)
        {
            return entities.FirstOrDefault(x => x.Id == id);
        }
    }
}