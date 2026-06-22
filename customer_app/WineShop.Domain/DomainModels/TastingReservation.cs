using System.ComponentModel.DataAnnotations;
using WineShop.Domain.Constants;
using WineShop.Domain.Identity;

namespace WineShop.Domain.DomainModels
{
    public class TastingReservation : BaseEntity
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime ReservationStart { get; set; }

        [Required]
        public DateTime ReservationEnd { get; set; }

        [Range(1, 8)]
        public int NumberOfGuests { get; set; }

        [Required]
        public Guid TastingPackageId { get; set; }
        public virtual TastingPackage TastingPackage { get; set; }

        [Required]
        public string Status { get; set; } = ReservationStatuses.Confirmed; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; }
        public virtual EShopApplicationUser User { get; set; }
    }
}