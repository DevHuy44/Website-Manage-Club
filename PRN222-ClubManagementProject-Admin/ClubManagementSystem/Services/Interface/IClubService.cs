using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IClubService
    {
        Task AddClubAsync(Club club);
        Task<IEnumerable<Club>> GetAllClubsAsync();       
        Task<IEnumerable<Club>> GetAllClubsApprovedAsync();       
        Task<ClubDetailsViewDto> GetClubDetailsAsync(int clubId, int postNumber, int postSize);
        Task<(bool success, string message)> UpdateClubAsync(ClubEditDto clubEditDto);
        Task<Club> GetClubByClubIdAsync(int clubId);
        Task<Club> GetClubAsync(int id);
        Task<(bool success, string message)> DeleteClub(Club club);
        Task<Club> CheckClubName(string clubName);
    }
}
