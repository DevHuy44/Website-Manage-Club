using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;

namespace Services.Interface
{
    public interface IClubRequestService
    {
        Task<ClubRequest?> AddClubRequestAsync(ClubRequest clubRequest);
        Task <IEnumerable<ClubRequest?>> GetAllClubRequestAsync(string role, int userId);
        Task<ClubRequest?> GetClubRequestById(int id);
        Task UpdateClubRequestStatus(ClubRequest clubRequest);

    }
}
