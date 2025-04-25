using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ClubTaskService : IClubTaskService
    {
        private readonly IClubTaskRepository _repository;

        public ClubTaskService(IClubTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClubTask> AddClubTaskAsync(ClubTask task)
        {
            return await _repository.AddClubTaskAsync(task);
        }

        public async Task<ClubTask> GetClubTask(int taskID)
        {
            return await _repository.GetClubTaskAsync(taskID);
        }

        public async Task<IEnumerable<ClubTask>> GetClubTasksAsync(int clubID)
        {
            return (await _repository.GetClubTasksAsync())
                .Where(a => a.Status != "Cancelled")
                .Where(t => t.CreatedByNavigation.ClubId == clubID);
        }

        public async Task<IEnumerable<ClubTask>> GetClubTasksAsync(int clubID, int eventID)
        {
            return (await _repository.GetClubTasksAsync())
                .Where(a => a.Status != "Cancelled")
                .Where(t => t.EventId == eventID)
                .Where(t => t.CreatedByNavigation.ClubId == clubID);
        }

        public async Task<bool> UpdateClubTaskAsync(ClubTask task)
        {
            return await _repository.UpdateClubTaskAsync(task);
        }

        public async Task<IEnumerable<ClubTask>> GetPersonalClubTasksAsync(int userID)
        {
            return (await _repository.GetClubTasksAsync())
                .Where(a => a.Status != "Cancelled")
                .Where(t => t.TaskAssignments
                    .Where(u => u.Membership.UserId == userID)
                    .Any());
        }

        public async Task<bool> IsAssigned(int taskID)
        {
            return (await _repository.GetClubTaskAsync(taskID))
                .TaskAssignments
                .Where(a => a.Status != "Cancelled")
                .Any();
        }

        public Task<IEnumerable<ClubTask>> GetClubTasksAsync()
        {
            return _repository.GetClubTasksAsync();
        }

        public async Task<bool> IsCompleted(int taskID)
        {
            var task = (await _repository.GetClubTaskAsync(taskID)).TaskAssignments
                .Select(t => t.Status).ToList();
            return !task.Contains("On Going");
        }
    }
}
