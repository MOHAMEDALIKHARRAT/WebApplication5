using WebApplication5.Models;
using WebApplication5.Repository;

namespace WebApplication5.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task SendNotificationAsync(string recipientId, string message)
        {
            var notification = new Notification
            {
                RecipientId = recipientId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
        }

    }
}
