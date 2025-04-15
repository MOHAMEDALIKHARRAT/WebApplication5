using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string RecipientId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
