using Microsoft.EntityFrameworkCore;
using WineShop.Domain.DomainModels;
using WineShop.Repository.Interface;

namespace WineShop.Repository.Implementation
{
    public class TastingReservationRepository : ITastingReservationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TastingReservation> entities;

        public TastingReservationRepository(ApplicationDbContext context)
        {
            _context = context;
            entities = context.Set<TastingReservation>();
        }

        public List<TastingReservation> GetAllReservations()
        {
            return entities
                .Include(x => x.TastingPackage)
                .Include(x => x.User)
                .ToList();
        }

        public TastingReservation? GetById(Guid id)
        {
            return entities
                .Include(x => x.TastingPackage)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);
        }

        public TastingReservation? GetByIdForUser(Guid id, string userId)
        {
            return entities
                .Include(x => x.TastingPackage)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public List<TastingReservation> GetReservationsForUser(string userId)
        {
            return entities
                .Include(x => x.TastingPackage)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ReservationStart)
                .ToList();
        }

        public List<TastingReservation> GetReservationsForDate(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return entities
                .Include(x => x.TastingPackage)
                .Include(x => x.User)
                .Where(x => x.ReservationStart < endOfDay && x.ReservationEnd > startOfDay)
                .OrderBy(x => x.ReservationStart)
                .ToList();
        }

        public List<TastingReservation> GetOverlappingReservations(DateTime start, DateTime end)
        {
            return entities
                .Include(x => x.TastingPackage)
                .Include(x => x.User)
                .Where(x => x.ReservationStart < end && start < x.ReservationEnd)
                .ToList();
        }

        public void Insert(TastingReservation reservation)
        {
            entities.Add(reservation);
            _context.SaveChanges();
        }

        public void Update(TastingReservation reservation)
        {
            entities.Update(reservation);
            _context.SaveChanges();
        }

    }
}