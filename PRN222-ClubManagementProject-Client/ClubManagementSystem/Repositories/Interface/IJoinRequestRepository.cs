using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IJoinRequestRepository
    {
        Task<IEnumerable<JoinRequest>> GetJoinRequestsAsync();
        Task<JoinRequest> GetJoinRequestAsync(int id);
        Task<JoinRequest> AddJoinRequestAsync(JoinRequest joinRequest);
        Task<bool> DeleteJoinRequestAsync(int id);
        Task<bool> UpdateJoinRequestAsync(JoinRequest joinRequest);
    }
}
