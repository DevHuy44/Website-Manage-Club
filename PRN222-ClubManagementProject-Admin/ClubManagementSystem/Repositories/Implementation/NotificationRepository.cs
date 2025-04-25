using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class NotificationRepository : INotificationRepository
    {
        private FptclubsContext _context;

        public NotificationRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            _context.Notifications.Remove(await GetNotificationAsync(notificationId));
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Notification> GetNotificationAsync(int notificationId)
        {
            return await _context.Notifications.FirstOrDefaultAsync(noti => noti.NotificationId == notificationId)?? new Notification() {Message = "null" };
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync()
        {
            return await _context.Notifications.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<Boolean> UpdateNotificationAsync(Notification notification)
        {
            _context.Entry<Notification>(notification).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
