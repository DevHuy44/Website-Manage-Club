using ClubManagementSystem.Services;
using Polly.Retry;
using Polly;

namespace ClubManagementSystem.Controllers.Worker
{
    public class EmailWorker : BackgroundService
    {
        private readonly IQueueService _queueService;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailWorker> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public EmailWorker(
            IQueueService queueService,
            IEmailService emailService,
            ILogger<EmailWorker> logger)
        {
            _queueService = queueService;
            _emailService = emailService;
            _logger = logger;

            // Cấu hình retry policy với Polly
            _retryPolicy = Policy
                .Handle<Exception>() // Retry khi có bất kỳ lỗi nào
                .WaitAndRetryAsync(
                    retryCount: 3, // Thử lại 3 lần
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Chờ 2s, 4s, 8s (tăng lũy thừa)
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            $"Retry {retryCount} after {timeSpan.TotalSeconds}s due to error: {exception.Message}");
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var emailItem = await _queueService.DequeueEmailAsync();
                    if (emailItem.HasValue)
                    {
                        var (email, fullName, emailType) = emailItem.Value;

                        await _retryPolicy.ExecuteAsync(async () =>
                        {
                            //_logger.LogInformation($"Attempting to send {emailType} email to {email} for booking {booking.BookingId}");

                            if (emailType == "remind")
                            {
                                await _emailService.SendRemindFeesEmailAsync(email, fullName);
                                _logger.LogInformation($"Confirmation email successfully sent to {email}");
                            }
                            else if (emailType == "payment")
                            {
                                await _emailService.SendPaymentSuccessEmailAsync(email, fullName);
                                _logger.LogInformation($"Confirmation email successfully sent to {email}");
                            }
                            else if (emailType == "expired")
                            {
                                await _emailService.SendRemindExpiredEmailAsync(email, fullName);
                                _logger.LogInformation($"Confirmation email successfully sent to {email}");
                            }
                            else
                            {
                                _logger.LogWarning($"Unknown email type: {emailType} for email {email}");
                            }
                        });
                    }
                    else
                    {
                        // Không có email trong hàng đợi, chờ 1 giây trước khi kiểm tra lại
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // Nếu retry vẫn thất bại sau 3 lần
                    _logger.LogError(ex, "Failed to send email after all retries.");
                    await Task.Delay(5000, stoppingToken); // Chờ 5 giây trước khi thử email tiếp theo
                }
            }

            _logger.LogInformation("Email Worker stopped.");
        }
    }
}
