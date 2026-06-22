using System.ComponentModel.DataAnnotations;

namespace WineShop.Domain.DomainModels
{
    public class TastingPackage : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, 24)]
        public int DurationHours { get; set; }

        [Range(1, 8)]
        public int MaxGuests { get; set; } = 8;

        [Range(1, int.MaxValue)]
        public int Price { get; set; }

        public bool BlocksWholeDay { get; set; }

        public virtual ICollection<TastingReservation> Reservations { get; set; } = new List<TastingReservation>();
    }
}