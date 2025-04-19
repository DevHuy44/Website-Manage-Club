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
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly ITaskAssignmentRepository _repository;

        public TaskAssignmentService(ITaskAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskAssignment> AddTaskAssignmentAsync(TaskAssignment task)
        {
            return await _repository.AddTaskAssignmentAsync(task);
        }

        public async Task<IEnumerable<TaskAssignment>> GetPersonalTaskAssignmentsAsync(int userID)
        {
            return (await _repository.GetTaskAssignmentsAsync())
                .Where(a => a.Membership.UserId == userID);
        }

        public async Task<TaskAssignment> GetTaskAssignmentAsync(int assignmentID)
        {
            return await _repository.GetTaskAssignmentAsync(assignmentID);
        }

        public async Task<TaskAssignment> GetTaskAssignmentAsync(int taskID, int memberID)
        {
            return (await _repository.GetTaskAssignmentsAsync())
                .Where(a => a.MembershipId == memberID && a.TaskId == taskID)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<TaskAssignment>> GetTaskAssignmentsAsync(int taskID)
        {
            return (await _repository.GetTaskAssignmentsAsync())
                .Where(a => a.TaskId == taskID);
        }

        public async Task<bool> IsAssignedBefore(int memberID, int taskID)
        {
            return (await _repository.GetTaskAssignmentsAsync())
                .Where(a => a.Status == "On Going" || a.Status == "Pending")
                .Where(a => a.MembershipId == memberID && a.TaskId == taskID)
                .Any();
        }

        public async Task<bool> UpdateTaskAssignmentAsync(TaskAssignment task)
        {
            return await _repository.UpdateTaskAssignmentAsync(task);
        }
    }
}
