using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string OrderRef { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        [ForeignKey("VisitId")]
        public Visit Visit { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new();

    }
}
