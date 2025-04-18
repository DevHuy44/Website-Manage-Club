using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class JoinRequestService : IJoinRequestService
    {
        private readonly IJoinRequestRepository _joinRequestRepository;

        public JoinRequestService(IJoinRequestRepository joinRequestRepository)
        {
            _joinRequestRepository = joinRequestRepository;
        }

        public async Task<JoinRequest> AddJoinRequestAsync(JoinRequest joinRequest)
        {
            return await _joinRequestRepository.AddJoinRequestAsync(joinRequest);
        }

        public async Task<JoinRequest> GetJoinRequestAsync(int id)
        {
            return await _joinRequestRepository.GetJoinRequestAsync(id);
        }

        public async Task<IEnumerable<JoinRequest>> GetJoinRequestsAsync(int ClubID)
        {
            return (await _joinRequestRepository.GetJoinRequestsAsync()).Where(j => j.ClubId == ClubID).Where(j => j.Status == "Pending");
        }

        public Task<bool> UpdateJoinRequestAsync(JoinRequest joinRequest)
        {
            return _joinRequestRepository.UpdateJoinRequestAsync(joinRequest);
        }

        public async Task<bool> isRequested(int ClubID, int userID)
        {
            return (await _joinRequestRepository.GetJoinRequestsAsync())
                .Where(j => j.ClubId == ClubID && j.UserId == userID)
                .Where(j => j.Status == "Pending")
                .Any();
        }
    }
}
