namespace ClubManagementSystem.Services
{
    public interface IQueueService
    {
        void EnqueueEmail(string email, string fullName, string emailType);
        Task<(string email, string fullName, string emailType)?> DequeueEmailAsync();
    }
}
