using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class CompetitorProduct
    {
        [Key]
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("VisitId")]
        public Visit Visit { get; set; }

    }
}
