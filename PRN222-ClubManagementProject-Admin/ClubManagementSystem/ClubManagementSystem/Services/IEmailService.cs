namespace ClubManagementSystem.Services
{
    public interface IEmailService
    {
        Task<bool> SendRemindFeesEmailAsync(string toEmail, string fullName);
        //Task<bool> SendConfirmAccountEmailAsync(string toEmail, string fullName, string confirmationLink);
        Task<bool> SendPaymentSuccessEmailAsync(string toEmail, string fullName);
        Task<bool> SendRemindExpiredEmailAsync(string toEmail, string fullName);
    }
}
