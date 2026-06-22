using System.ComponentModel.DataAnnotations;

namespace WineShop.Domain.DomainModels
{
    public class ProductAward : BaseEntity
    {
        [Required]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public string CompetitionName { get; set; }

        [Required]
        public string AwardName { get; set; } // Gold, Silver, Bronze, Grand Gold...

        public int? AwardYear { get; set; }
    }
}