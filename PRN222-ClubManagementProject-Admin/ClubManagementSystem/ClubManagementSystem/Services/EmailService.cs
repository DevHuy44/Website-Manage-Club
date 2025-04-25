using ClubManagementSystem.Utilities;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ClubManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendRemindFeesEmailAsync(string toEmail, string fullName)
        {
            string subject = "Nhắc nhở nộp phí";
            string body = EmailTemplates.GetRemindFeesTemplate(fullName);
            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendPaymentSuccessEmailAsync(string toEmail, string fullName)
        {
            string subject = "Thanh toán thành công";
            string body = EmailTemplates.GetPaymentSuccessTemplate(fullName);
            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendRemindExpiredEmailAsync(string toEmail, string fullName)
        {
            string subject = "Hết hạn nộp phí CLB";
            string body = EmailTemplates.GetRemindExpiredTemplate(fullName);
            return await SendEmailAsync(toEmail, subject, body);
        }

        //public async Task<bool> SendConfirmAccountEmailAsync(string toEmail, string fullName, string confirmationLink)
        //{
        //    string subject = "Xác nhận tài khoản của bạn";
        //    string body = EmailTemplates.GetConfirmationAccountTemplate(fullName, confirmationLink);
        //    return await SendEmailAsync(toEmail, subject, body);
        //}


        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = _emailSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
