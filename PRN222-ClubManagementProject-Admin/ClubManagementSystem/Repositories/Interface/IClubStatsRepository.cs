using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IClubStatsRepository
    {
        Task<List<object>> GetMembersByClubAsync(DateTime startDate, DateTime endDate);
        Task<List<object>> GetEventsByClubAsync(DateTime startDate, DateTime endDate);
        Task<List<object>> GetPostsByClubAsync(DateTime startDate, DateTime endDate);
    }
}
