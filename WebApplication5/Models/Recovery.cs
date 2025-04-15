using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Recovery
    {

        [Key]
        public int Id { get; set; }
        public int VisitId { get; set; }
        public decimal AmountCollected { get; set; }
        public DateTime CollectionDate { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("VisitId")]
        public Visit Visit { get; set; }

    }
}
