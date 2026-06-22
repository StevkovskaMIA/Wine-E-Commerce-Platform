namespace WineShop.Domain.DTO
{
    public class ProductFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Color { get; set; }

        public string? PriceSort { get; set; }   // priceAsc, priceDesc
        public string? YearSort { get; set; }    // yearDesc, yearAsc

        public string? AwardName { get; set; }
        public bool OnlyAvailable { get; set; }
    }
}