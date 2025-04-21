using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;

namespace Services.Implementation
{
    public class ClubRequestService : IClubRequestService
    {
        private readonly IClubRequestRepository _clubRequestRepository;
        public ClubRequestService(IClubRequestRepository clubRequestRepository)
        {
            _clubRequestRepository = clubRequestRepository;
        }
        public async Task<ClubRequest?> AddClubRequestAsync(ClubRequest clubRequest)
        {
            return await _clubRequestRepository.AddClubRequestAsync(clubRequest);
        }
        public async Task <IEnumerable<ClubRequest?>> GetAllClubRequestAsync(string role,int userId)
        {
            if (role.Equals("SystemAdmin"))
            {
                return await _clubRequestRepository.GetAllClubRequestPendingAsync();
            }
            return await _clubRequestRepository.GetAllClubRequestWithUserId(userId);
        }

        public async Task<ClubRequest?> GetClubRequestById(int id)
        {
            return await _clubRequestRepository.GetClubRequestById(id);
        }

        public async Task UpdateClubRequestStatus(ClubRequest clubRequest)
        {
            await _clubRequestRepository.UpdateStatus(clubRequest);
        }
    }
}
