using BussinessObjects.Models;
using ClubManagementSystem.Controllers.Hubs;
using ClubManagementSystem.Services;
using Cronos;
using Microsoft.AspNetCore.SignalR;
using Repositories.Interface;

namespace ClubManagementSystem.Controllers.Worker
{
    public class MembershipFeeReminderWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MembershipFeeReminderWorker> _logger;
        private readonly CronExpression _cronExpression;
        private readonly IQueueService _queueService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MembershipFeeReminderWorker(IServiceProvider serviceProvider, ILogger<MembershipFeeReminderWorker> logger, IQueueService queueService, IHubContext<NotificationHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;
            _cronExpression = CronExpression.Parse("*/1 * * * *"); // Chạy lúc 00:00 mỗi ngày
            _queueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = _cronExpression.GetNextOccurrence(now)?.ToLocalTime() ?? now.AddMinutes(1);

                var delay = nextRun - DateTime.Now;
                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                await ProcessMembershipFeesAsync(stoppingToken);
            }
        }

        private async Task ProcessMembershipFeesAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IMembershipFeeRepository>();

                // 1. Nhắc nhở trước 1 ngày
                var tomorrow = DateTime.Today.AddDays(1);
                var feesToRemind = await repository.GetMembershipFeesByDueDateAsync(tomorrow, "Pending");
                foreach (var fee in feesToRemind)
                {
                    var notification = new Notification
                    {
                        UserId = fee.ClubMember.UserId,
                        Message = $"Nhắc nhở: Khoản phí '{fee.Fee.FeeDescription}' trị giá {fee.Fee.Amount} VND sẽ đến hạn vào {fee.Fee.DueDate:dd/MM/yyyy}. Vui lòng thanh toán đúng hạn.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await repository.AddNotificationAsync(notification);
                    await _hubContext.Clients.User(fee.ClubMember.UserId.ToString()).SendAsync("ReceiveNotification", notification.Message);
                    //await _hubContext.SendNotification(fee.ClubMember.UserId, notification.Message);
                    // Gửi email (tùy chọn)
                    _queueService.EnqueueEmail(fee.ClubMember.User.Email, fee.ClubMember.User.Username, "remind");
                }

                // 2. Cập nhật trạng thái Overdue
                var overdueFees = await repository.GetMembershipFeesByOverDateAsync("Pending");
                foreach (var fee in overdueFees)
                {
                    if (fee.Fee.DueDate <= DateTime.Today)
                    {
                        fee.Status = "Overdue";
                        await repository.UpdateMembershipFeeAsync(fee);

                        var notification = new Notification
                        {
                            UserId = fee.ClubMember.UserId,
                            Message = $"Khoản phí '{fee.Fee.FeeDescription}' trị giá {fee.Fee.Amount} VND đã quá hạn. Vui lòng thanh toán sớm nhất có thể.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };
                        await repository.AddNotificationAsync(notification);
                        await _hubContext.Clients.User(fee.ClubMember.UserId.ToString()).SendAsync("ReceiveNotification", notification.Message);
                        //await _hubContext.SendNotification(fee.ClubMember.UserId, notification.Message);
                        // Gửi email (tùy chọn)
                        _queueService.EnqueueEmail(fee.ClubMember.User.Email, fee.ClubMember.User.Username, "expired");
                    }
                }

                _logger.LogInformation("Processed membership fees at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing membership fees at {Time}", DateTime.Now);
            }
        }
    }
}
