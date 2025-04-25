using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IJoinRequestService
    {
        Task<IEnumerable<JoinRequest>> GetJoinRequestsAsync(int ClubID);
        Task<JoinRequest> AddJoinRequestAsync(JoinRequest joinRequest);
        Task<JoinRequest> GetJoinRequestAsync(int id);
        Task<bool> UpdateJoinRequestAsync(JoinRequest joinRequest);
        Task<bool> isRequested(int ClubID, int userID);
    }
}
