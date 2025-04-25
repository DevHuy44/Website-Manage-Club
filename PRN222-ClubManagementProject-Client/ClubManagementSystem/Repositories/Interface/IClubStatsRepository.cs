using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IClubStatsRepository
    {
        Task<List<(string ClubName, int MemberCount)>> GetMembersByClubAsync(DateTime startDate, DateTime endDate);
        Task<List<(string ClubName, int EventCount)>> GetEventsByClubAsync(DateTime startDate, DateTime endDate);
        Task<List<(string ClubName, int PostCount)>> GetPostsByClubAsync(DateTime startDate, DateTime endDate);
    }
}
