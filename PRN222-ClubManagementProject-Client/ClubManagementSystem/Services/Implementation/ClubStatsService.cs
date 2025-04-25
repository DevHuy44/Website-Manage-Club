using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ClubStatsService : IClubStatsService
    {
        private readonly IClubStatsRepository _repository;

        public ClubStatsService(IClubStatsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<(string ClubName, int MemberCount)>> GetMemberStatsAsync(DateTime targetMonth)
        {
            var startDate = targetMonth;
            var endDate = targetMonth.AddMonths(1);
            return await _repository.GetMembersByClubAsync(startDate, endDate);
        }

        public async Task<List<(string ClubName, int EventCount)>> GetEventStatsAsync(DateTime targetMonth)
        {
            var startDate = targetMonth;
            var endDate = targetMonth.AddMonths(1);
            return await _repository.GetEventsByClubAsync(startDate, endDate);
        }

        public async Task<List<(string ClubName, int PostCount)>> GetPostStatsAsync(DateTime targetMonth)
        {
            var startDate = targetMonth;
            var endDate = targetMonth.AddMonths(1);
            return await _repository.GetPostsByClubAsync(startDate, endDate);
        }
    }
}
