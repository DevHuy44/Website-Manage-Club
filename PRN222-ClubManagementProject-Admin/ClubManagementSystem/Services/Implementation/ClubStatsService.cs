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

        public async Task<List<object>> GetMemberStatsAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);
            return await _repository.GetMembersByClubAsync(startDate, endDate);
        }

        public async Task<List<object>> GetEventStatsAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);
            return await _repository.GetEventsByClubAsync(startDate, endDate);
        }

        public async Task<List<object>> GetPostStatsAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);
            return await _repository.GetPostsByClubAsync(startDate, endDate);
        }
    }
}
