using Microsoft.AspNetCore.SignalR;

namespace ClubManagementSystem.Controllers.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
