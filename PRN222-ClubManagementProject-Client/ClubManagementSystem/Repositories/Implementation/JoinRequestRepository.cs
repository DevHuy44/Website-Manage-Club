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
    public class JoinRequestRepository : IJoinRequestRepository
    {
        private readonly FptclubsContext _context;

        public JoinRequestRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<JoinRequest> AddJoinRequestAsync(JoinRequest joinRequest)
        {
            await _context.JoinRequests.AddAsync(joinRequest);
            await _context.SaveChangesAsync();
            return joinRequest;
        }

        public async Task<bool> DeleteJoinRequestAsync(int id)
        {
            _context.JoinRequests.Remove(await GetJoinRequestAsync(id));
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<JoinRequest> GetJoinRequestAsync(int id)
        {
            return await _context.JoinRequests.Include(j => j.User).Include(j => j.Club).FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<IEnumerable<JoinRequest>> GetJoinRequestsAsync()
        {
            return await _context.JoinRequests.Include(j => j.User).Include(j => j.Club).ToListAsync();
        }

        public async Task<bool> UpdateJoinRequestAsync(JoinRequest joinRequest)
        {
            _context.Entry<JoinRequest>(joinRequest).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
