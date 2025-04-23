using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITaskAssignmentService
    {
        Task<TaskAssignment> AddTaskAssignmentAsync(TaskAssignment task);
        Task<TaskAssignment> GetTaskAssignmentAsync(int assignmentID);
        Task<TaskAssignment> GetTaskAssignmentAsync(int taskID, int memberID);
        Task<IEnumerable<TaskAssignment>> GetTaskAssignmentsAsync(int taskID);
        Task<IEnumerable<TaskAssignment>> GetPersonalTaskAssignmentsAsync(int userID);
        Task<bool> UpdateTaskAssignmentAsync(TaskAssignment task);
        Task<bool> IsAssignedBefore(int memberID, int taskID);
    }
}
