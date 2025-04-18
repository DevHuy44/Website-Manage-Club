using BussinessObjects.Models;
using ClubManagementSystem.Controllers.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClubManagementSystem.Controllers.Worker
{
    public class StatusUpdatingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StatusUpdatingWorker> _logger;

        public StatusUpdatingWorker(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, ILogger<StatusUpdatingWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Worker running at {DateTime.Now}");

                    using (var scopeServices = _serviceScopeFactory.CreateScope())
                    {
                        var notiService = scopeServices.ServiceProvider.GetRequiredService<INotificationService>();
 
                        var sender = scopeServices.ServiceProvider.GetRequiredService<SignalRSender>();

                       

                        var NotiExpirationLimit = _configuration["Notification:ExpiredDate"];
                        _logger.LogInformation($"Expiration time {NotiExpirationLimit}");
                        if (NotiExpirationLimit != null)
                        {
                            if (int.TryParse(NotiExpirationLimit, out int expirationLimit))
                            {
                                var notis = await notiService.GetExpiredNotificationsAsync(expirationLimit);
                                foreach (var noti in notis)
                                {
                                    await notiService.DeleteNotificationAsync(noti.NotificationId);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Invalid NotiExpirationLimit value in configuration.");
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the StatusUpdatingWorker.");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

    }
}
