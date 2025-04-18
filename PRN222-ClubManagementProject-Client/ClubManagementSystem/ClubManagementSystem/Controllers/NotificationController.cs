using BussinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using ClubManagementSystem.Controllers.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace ClubManagementSystem.Controllers
{
    [Route("Notification")]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _NS;
        private readonly IConnectionService _CS;
        private readonly IHubContext<ServerHub> _hubContext;

        public NotificationController(INotificationService NS, IConnectionService CS, IHubContext<ServerHub> hubContext, SignalRSender sender)
        {
            _NS = NS;
            _CS = CS;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifications = await _NS.GetNotificationsAsync(Int32.Parse(UserID));
            return View(notifications);
        }

        [HttpPost("UpdateAsync")]
        public async Task<JsonResult> UpdateAsync(int NotiID) 
        {
            var notification = await _NS.GetNotificationAsync(NotiID);
            notification.IsRead = true;
            await _NS.UpdateNotificationAsync(notification);
            return Json(notification);
        }

        [HttpPost("UpdateAllAsync")]
        public async Task<JsonResult> UpdateAllAsync()
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _NS.UpdateAllNotificationsAsync(Int32.Parse(UserID));
            var notifications = await _NS.GetNotificationsAsync(Int32.Parse(UserID));
            return Json(notifications);
        }
    }
}
