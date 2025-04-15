namespace WebApplication5.Dto
{
    public class OrderDto
    {
        public int VisitId { get; set; }
        public string OrderRef { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<OrderLineDto> OrderLines { get; set; } = new();

    }
}
