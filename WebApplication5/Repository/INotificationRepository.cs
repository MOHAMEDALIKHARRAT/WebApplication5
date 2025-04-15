using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<List<Notification>> GetByRecipientAsync(string recipientId);
        Task MarkAsReadAsync(int notificationId);

    }
}
