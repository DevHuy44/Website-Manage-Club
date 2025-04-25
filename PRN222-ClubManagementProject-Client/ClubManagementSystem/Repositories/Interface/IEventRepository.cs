using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEventsAsync();
        Task<Event> GetEventAsync(int eventId);
        Task<Event> AddEventAsync(Event Event);
        Task<Boolean> UpdateEventAsync(Event Event);
        Task<Boolean> DeleteEventAsync(int eventId);
    }
}
