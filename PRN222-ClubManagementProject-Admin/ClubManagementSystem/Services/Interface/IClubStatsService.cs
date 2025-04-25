using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IClubStatsService
    {
        Task<List<object>> GetMemberStatsAsync(DateTime month);
        Task<List<object>> GetEventStatsAsync(DateTime month);
        Task<List<object>> GetPostStatsAsync(DateTime month);
    }
}
