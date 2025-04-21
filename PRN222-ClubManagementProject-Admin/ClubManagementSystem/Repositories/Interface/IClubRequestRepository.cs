using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;

namespace Repositories.Interface
{
    public interface IClubRequestRepository
    {
        Task<ClubRequest?> AddClubRequestAsync(ClubRequest clubRequest);
        Task<IEnumerable<ClubRequest?>> GetAllClubRequestPendingAsync();

        Task<IEnumerable<ClubRequest?>> GetAllClubRequestWithUserId(int userId);
        Task<ClubRequest?> GetClubRequestById(int id);
        Task UpdateStatus(ClubRequest clubRequest);
    }
}
