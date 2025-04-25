using System.Collections.Concurrent;

namespace ClubManagementSystem.Services
{
    public class InMemoryQueueService : IQueueService
    {
        private readonly ConcurrentQueue<(string email, string fullName, string emailType)> _queue = new();

        public void EnqueueEmail(string email, string fullName, string emailType)
        {
            _queue.Enqueue((email, fullName, emailType));
        }

        public async Task<(string email, string fullName, string emailType)?> DequeueEmailAsync()
        {
            if (_queue.TryDequeue(out var item))
            {
                return item;
            }
            return null;
        }
    }
}
