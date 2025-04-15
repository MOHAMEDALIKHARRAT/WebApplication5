namespace WebApplication5.Dto
{
    public class VisitOrderItemDto
    {
        public int ArticleId { get; set; }
        public string ArticleCode { get; set; } = string.Empty;
        public string ArticleDesignation { get; set; } = string.Empty;
        public decimal UnitPriceHT { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal TotalHT => Quantity * UnitPriceHT - Discount;
        public decimal TotalTTC => TotalHT * 1.19m; // Assuming 19% VAT

    }
}
