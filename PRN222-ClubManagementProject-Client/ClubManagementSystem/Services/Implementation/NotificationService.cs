using BussinessObjects.Models;
using Repositories.Implementation;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository) 
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> Notify(int userID, string mess, string location)
        {
            Notification noti  = new Notification()
            {
                CreatedAt = DateTime.Now,
                IsRead = false,
                Location = location,
                Message = mess,
                UserId = userID
            };
            return await _notificationRepository.AddNotificationAsync(noti);
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            return await _notificationRepository.AddNotificationAsync(notification);
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            return await _notificationRepository.DeleteNotificationAsync(notificationId);
        }

        public async Task<Notification> GetNotificationAsync(int notificationId)
        {
            return await _notificationRepository.GetNotificationAsync(notificationId);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync(int userId)
        {
            return (await _notificationRepository.GetNotificationsAsync())
                .Where(n => n.UserId == userId);
        }

        public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            return await _notificationRepository.UpdateNotificationAsync(notification);
        }

        public async Task<bool> UpdateAllNotificationsAsync(int userId)
        {
            var isUpdated = true;
            var notis = await GetNotificationsAsync(userId);
            foreach (var notification in notis)
            {
                notification.IsRead = true;
                isUpdated = await _notificationRepository.UpdateNotificationAsync(notification)? isUpdated : false;
            }
            return isUpdated;
        }

        public async Task<IEnumerable<Notification>> GetExpiredNotificationsAsync(int days)
        {
            return (await _notificationRepository.GetNotificationsAsync())
                .Where(n => n.CreatedAt <= DateTime.Now.Date.AddDays(days*(-1)));
        }
    }
}
