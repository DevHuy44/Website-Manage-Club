using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Azure;
using Services.Interface;

namespace ClubManagementSystem.Controllers.SignalR
{
    public class SignalRSender
    {
        private readonly IHubContext<ServerHub> _hubContext;
        private readonly IConnectionService _CS;
        private readonly INotificationService _notificationService;

        public SignalRSender(IHubContext<ServerHub> hubContext, IConnectionService CS, INotificationService notificationService)
        {
            _hubContext = hubContext;
            _CS = CS;
            _notificationService = notificationService;
        }

        public async Task Notify(Notification noti, int userID)
        {
            var connection = await _CS.GetConnection(userID);
            await _notificationService.AddNotificationAsync(noti);
            if (connection != null)
            {
                await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("notifyNews", noti);
            }
            //await _hubContext.Clients.All.SendAsync("notifyNews", noti);
        }

        public async Task NotifyPost(CommentDto? comment, PostReaction? reaction)
        {
            await _hubContext.Clients.All.SendAsync("notifyPost", comment, reaction);
        }

        public async Task NotifyDeletePost(Comment? comment, PostReaction? reaction)
        {
            await _hubContext.Clients.All.SendAsync("notifyDeletePost", comment, reaction);
        }
    }
}
