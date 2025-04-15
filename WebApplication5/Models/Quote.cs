using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string QuoteRef { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public DateTime QuoteDate { get; set; }
        [ForeignKey("VisitId")]
        public Visit Visit { get; set; }
        public List<QuoteLine> QuoteLines { get; set; } = new();

    }
}
