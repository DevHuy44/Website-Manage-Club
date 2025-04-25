using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IClubStatsService
    {
        Task<List<(string ClubName, int MemberCount)>> GetMemberStatsAsync(DateTime targetMonth);
        Task<List<(string ClubName, int EventCount)>> GetEventStatsAsync(DateTime targetMonth);
        Task<List<(string ClubName, int PostCount)>> GetPostStatsAsync(DateTime targetMonth);
    }
}
