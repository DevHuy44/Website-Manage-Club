using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;

namespace Services.Interface
{
    public interface INotificationService
    {
        Task<Notification> Notify(int userID, string mess, string location);
        Task<IEnumerable<Notification>> GetNotificationsAsync(int userId);
        Task<Notification> GetNotificationAsync(int notificationId);
        Task<Notification> AddNotificationAsync(Notification notification);
        Task<Boolean> UpdateNotificationAsync(Notification notification);
        Task<Boolean> DeleteNotificationAsync(int notificationId);
        Task<Boolean> UpdateAllNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetExpiredNotificationsAsync(int days);
    }
}
