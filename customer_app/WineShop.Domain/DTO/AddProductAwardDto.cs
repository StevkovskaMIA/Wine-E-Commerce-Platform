using System.ComponentModel.DataAnnotations;

namespace WineShop.Domain.DTO
{
    public class AddProductAwardDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string CompetitionName { get; set; }

        [Required]
        public string AwardName { get; set; }

        public int? AwardYear { get; set; }
    }
}