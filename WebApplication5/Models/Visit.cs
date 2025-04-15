using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models
{
    public class Visit
    {
        [Key]
        public int Id { get; set; }
        public string CommercialCref { get; set; } = string.Empty;
        public int TiersId { get; set; }
        public DateTime VisitDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("CommercialCref")]
        public Commercial Commercial { get; set; }
        [ForeignKey("TiersId")]
        public Tiers Client { get; set; }
    }
}