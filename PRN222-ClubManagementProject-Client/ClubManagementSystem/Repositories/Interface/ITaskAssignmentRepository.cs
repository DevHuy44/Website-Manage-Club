using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ITaskAssignmentRepository
    {
        Task<IEnumerable<TaskAssignment>> GetTaskAssignmentsAsync();
        Task<TaskAssignment> GetTaskAssignmentAsync(int TaskAssignmentID);
        Task<TaskAssignment> AddTaskAssignmentAsync(TaskAssignment TaskAssignment);
        Task<Boolean> UpdateTaskAssignmentAsync(TaskAssignment TaskAssignment);
        Task<Boolean> DeleteTaskAssignmentAsync(int TaskAssignmentID);
    }
}
