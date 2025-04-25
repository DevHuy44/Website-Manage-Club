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
    public class EventRepository : IEventRepository
    {
        private FptclubsContext _context;

        public EventRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<Event> AddEventAsync(Event Event)
        {
            await _context.Events.AddAsync(Event);
            await _context.SaveChangesAsync();
            return Event;
        }

        public async Task<bool> DeleteEventAsync(int EventId)
        {
            _context.Events.Remove(await GetEventAsync(EventId));
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Event> GetEventAsync(int EventId)
        {
            return await _context.Events
                .Include(e => e.CreatedByNavigation)
                .ThenInclude(e => e.User)
                .Include(e => e.CreatedByNavigation)
                    .ThenInclude(e => e.Club)
                .Include(e => e.Tasks)
                    .ThenInclude(e => e.TaskAssignments)
                .FirstOrDefaultAsync(noti => noti.EventId == EventId);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            return await _context.Events
                .OrderBy(e => e.EventDate)
                .Include(e => e.CreatedByNavigation)
                    .ThenInclude(e => e.User)
                .Include(e => e.CreatedByNavigation)
                    .ThenInclude(e => e.Club)
                .Include(e => e.Tasks)
                .ToListAsync();
        }

        public async Task<Boolean> UpdateEventAsync(Event Event)
        {
            _context.Entry<Event>(Event).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
