namespace WebApplication5.Dto
{
    public class VisitDto
    {
        public int Id { get; set; }
        public string CommercialCref { get; set; } = string.Empty;
        public int TiersId { get; set; }
        public DateTime VisitDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
    }
}