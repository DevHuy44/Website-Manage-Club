using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IClubRepository
    {
        Task AddClubAsync(Club club);
        Task<IEnumerable<Club>> GetAllClubAsync();
        Task<Club> GetClubByIdWithMembersPostsAsync(int clubId);
        Task<Club> UpdateClubAsync(Club club);
        Task<Club> GetClubByClubIdAsync(int clubId);
    }
}
