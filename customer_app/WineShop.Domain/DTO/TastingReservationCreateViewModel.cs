using System.ComponentModel.DataAnnotations;

namespace WineShop.Domain.DTO
{
    public class TastingReservationCreateViewModel
    {
        public Guid PackageId { get; set; }

        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public int DurationHours { get; set; }
        public int MaxGuests { get; set; }
        public int Price { get; set; }
        public bool BlocksWholeDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        public string? SelectedTime { get; set; }

        [Required]
        [Range(1, 8)]
        public int NumberOfGuests { get; set; } = 1;

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public List<string> AvailableTimes { get; set; } = new List<string>();
    }
}