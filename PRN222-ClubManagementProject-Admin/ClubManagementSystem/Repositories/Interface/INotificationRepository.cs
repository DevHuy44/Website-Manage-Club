using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;

namespace Repositories.Interface
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotificationsAsync();
        Task<Notification> GetNotificationAsync(int notificationId);
        Task<Notification> AddNotificationAsync(Notification notification);
        Task<Boolean> UpdateNotificationAsync(Notification notification);
        Task<Boolean> DeleteNotificationAsync(int notificationId);
    }
}
