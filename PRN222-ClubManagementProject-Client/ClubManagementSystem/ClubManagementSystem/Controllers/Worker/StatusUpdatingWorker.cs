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
                        var clubService = scopeServices.ServiceProvider.GetRequiredService<IClubService>();
                        var eventService = scopeServices.ServiceProvider.GetRequiredService<IEventService>();
                        var notiService = scopeServices.ServiceProvider.GetRequiredService<INotificationService>();
                        var memberService = scopeServices.ServiceProvider.GetRequiredService<IClubMemberService>();
                        var taskService = scopeServices.ServiceProvider.GetRequiredService<IClubTaskService>();
                        var sender = scopeServices.ServiceProvider.GetRequiredService<SignalRSender>();

                        var clubs = await clubService.GetAllClubsAsync();
                        foreach (var club in clubs)
                        {
                            var ongoing = await eventService.GetOnGoingEvent(club.ClubId);
                            if (ongoing != null)
                            {
                                ongoing.Status = "On Going";
                                await eventService.UpdateEventAsync(ongoing);

                                var members = await memberService.GetClubMembersAsync(ongoing.EventId);
                                foreach (var member in members)
                                {
                                    Notification newNoti = new Notification()
                                    {
                                        Message = $"Event {ongoing.EventTitle} is going on!",
                                        Location = $"{ongoing.CreatedByNavigation.Club.ClubName} Event",
                                        UserId = member.UserId
                                    };
                                    await sender.Notify(newNoti, newNoti.UserId);
                                }
                            }
                        }

                        var finishedEvents = await eventService.GetFinishedEvent();
                        foreach (var @event in finishedEvents)
                        {
                            @event.Status = "Finished";
                            await eventService.UpdateEventAsync(@event);
                        }

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

                        var tasks = await taskService.GetClubTasksAsync();
                        foreach (var task in tasks)
                        {
                            if(task.Status != "Completed")
                            {
                                task.Status = (await taskService.IsCompleted(task.TaskId)) ? "Completed" : "End";
                                await taskService.UpdateClubTaskAsync(task);
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
