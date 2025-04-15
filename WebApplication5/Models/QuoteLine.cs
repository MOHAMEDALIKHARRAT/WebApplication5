using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class QuoteLine
    {
        [Key]
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public int ArticleId { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [ForeignKey("QuoteId")]
        public Quote Quote { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }


    }
}
