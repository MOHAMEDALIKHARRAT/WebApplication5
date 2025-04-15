namespace WebApplication5.Dto
{
    public class QuoteDto
    {
        public int VisitId { get; set; }
        public string QuoteRef { get; set; } = string.Empty;
        public DateTime QuoteDate { get; set; }
        public List<QuoteLineDto> QuoteLines { get; set; } = new();

    }
}
