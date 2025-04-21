using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly FptclubsContext _context;

        public TaskAssignmentRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<TaskAssignment> AddTaskAssignmentAsync(TaskAssignment TaskAssignment)
        {
            await _context.TaskAssignments.AddAsync(TaskAssignment);
            await _context.SaveChangesAsync();
            return TaskAssignment;
        }

        public async Task<bool> DeleteTaskAssignmentAsync(int TaskAssignmentID)
        {
            var TaskAssignment = await GetTaskAssignmentAsync(TaskAssignmentID);
            _context.TaskAssignments.Remove(TaskAssignment);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<TaskAssignment> GetTaskAssignmentAsync(int TaskAssignmentID)
        {
            return await _context.TaskAssignments
                .Include(a => a.Membership)
                    .ThenInclude(a => a.User)
                .Include(a => a.Membership)
                    .ThenInclude(a => a.Club)
                .Include(a => a.Task)
                    .ThenInclude(a => a.Event)
                .FirstOrDefaultAsync(r => r.AssignmentId == TaskAssignmentID);
        }

        public async Task<IEnumerable<TaskAssignment>> GetTaskAssignmentsAsync()
        {
            return await _context.TaskAssignments
                .Include(a => a.Membership)
                    .ThenInclude(a => a.User)
                .Include(a => a.Membership)
                    .ThenInclude(a => a.Club)
                .Include(a => a.Task)
                    .ThenInclude(a => a.Event)
                .ToListAsync();
        }

        public async Task<bool> UpdateTaskAssignmentAsync(TaskAssignment TaskAssignment)
        {
            _context.Entry<TaskAssignment>(TaskAssignment).State = EntityState.Modified;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
