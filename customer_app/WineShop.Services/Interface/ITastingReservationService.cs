using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;

namespace WineShop.Service.Interface
{
    public interface ITastingReservationService
    {
        TastingReservationCreateViewModel? GetBookingModel(Guid packageId, string userId);
        List<string> GetAvailableTimeSlots(Guid packageId, DateTime date);
        bool CreateReservation(TastingReservationCreateViewModel model, string userId, out string message);
        List<TastingReservation> GetReservationsForUser(string userId);
        bool CancelReservation(Guid reservationId, string userId);
    }
}