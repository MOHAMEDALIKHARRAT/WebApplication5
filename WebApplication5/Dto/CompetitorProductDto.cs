namespace WebApplication5.Dto
{
    public class CompetitorProductDto
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }

    }
}
