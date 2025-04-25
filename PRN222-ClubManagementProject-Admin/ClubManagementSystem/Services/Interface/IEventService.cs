using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IEventService
    {
        public Task<Event> AddEventAsync(Event Event);
        public Task<bool> UpdateEventAsync(Event Event);
        public Task<IEnumerable<Event>> GetEventsAsync(int clubID);
        public Task<Event> GetEventAsync(int EventId);
        public Task<IEnumerable<Event>> GetEventsAsync(DateTime start, DateTime end, int clubID);
        public Task<bool> IsDependedOn(int EventID);
        public Task<bool> IsOccupied(DateTime eventDate, int clubID);
        public Task<IEnumerable<Event>> GetIncomingEvent(int clubID);
        public Task<Event> GetOnGoingEvent(int clubID);
        public Task<IEnumerable<Event>> GetFinishedEvent();
        public Task<IEnumerable<Event>> GetOnGoingEvents();
    }
}
