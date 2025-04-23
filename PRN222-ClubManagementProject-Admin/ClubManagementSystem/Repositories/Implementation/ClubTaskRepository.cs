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
    public class ClubTaskRepository : IClubTaskRepository
    {
        private readonly FptclubsContext _context;

        public ClubTaskRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<ClubTask> AddClubTaskAsync(ClubTask ClubTask)
        {
            await _context.Tasks.AddAsync(ClubTask);
            await _context.SaveChangesAsync();
            return ClubTask;
        }

        public async Task<bool> DeleteClubTaskAsync(int ClubTaskID)
        {
            var task = await GetClubTaskAsync(ClubTaskID);
            _context.Tasks.Remove(task);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<ClubTask> GetClubTaskAsync(int ClubTaskID)
        {
            return await _context.Tasks
                .Include(u => u.TaskAssignments
                    .Where(a => a.Status!="Declined" && a.Status!="Pending"))
                    .ThenInclude(u => u.Membership)
                        .ThenInclude(u => u.User)
                .Include(u => u.CreatedByNavigation)
                    .ThenInclude(u => u.Club)
                .Include(u => u.CreatedByNavigation)
                    .ThenInclude(u => u.User)
                .Include(u => u.Event)
                .FirstOrDefaultAsync(u => u.TaskId == ClubTaskID);
        }

        public async Task<IEnumerable<ClubTask>> GetClubTasksAsync()
        {
            return await _context.Tasks
                .Include(u => u.TaskAssignments
                    .Where(a => a.Status != "Declined" && a.Status != "Pending"))
                    .ThenInclude(u => u.Membership)
                        .ThenInclude(u => u.User)
                .Include(u => u.CreatedByNavigation)
                    .ThenInclude(u => u.Club)
                .Include(u => u.CreatedByNavigation)
                    .ThenInclude(u => u.User)
                .Include(u => u.Event)
                .ToListAsync();
        }

        public async Task<bool> UpdateClubTaskAsync(ClubTask ClubTask)
        {
            _context.Entry<ClubTask>(ClubTask).State = EntityState.Modified;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
