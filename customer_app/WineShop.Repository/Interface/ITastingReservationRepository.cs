using WineShop.Domain.DomainModels;

namespace WineShop.Repository.Interface
{
    public interface ITastingReservationRepository
    {
        List<TastingReservation> GetAllReservations();
        TastingReservation? GetById(Guid id);
        TastingReservation? GetByIdForUser(Guid id, string userId);
        List<TastingReservation> GetReservationsForUser(string userId);
        List<TastingReservation> GetReservationsForDate(DateTime date);
        List<TastingReservation> GetOverlappingReservations(DateTime start, DateTime end);
        void Insert(TastingReservation reservation);
        void Update(TastingReservation reservation);
    }
}