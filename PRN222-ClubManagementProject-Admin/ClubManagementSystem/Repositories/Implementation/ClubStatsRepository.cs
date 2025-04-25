using BussinessObjects.Models;
using Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class ClubStatsRepository : IClubStatsRepository
    {
        private readonly FptclubsContext _context ;

        public ClubStatsRepository(FptclubsContext context )
        {
            _context = context;
        }

        public async Task<List<object>> GetMembersByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from cm in _context.ClubMembers
                        where cm.JoinedAt != null && cm.JoinedAt >= startDate && cm.JoinedAt < endDate
                        group cm by cm.ClubId into g
                        select new
                        {
                            ClubId = g.Key,
                            MemberCount = g.Count()
                        };

            var result = await query
                .Join(_context.Clubs,
                      g => g.ClubId,
                      club => club.ClubId,
                      (g, club) => new { ClubName = club.ClubName, Count = g.MemberCount })
                .OrderByDescending(x => x.Count)
                .Select(x => new { item1 = x.ClubName, item2 = x.Count })
                .ToListAsync();

            return result.Cast<object>().ToList();
        }

        public async Task<List<object>> GetEventsByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from e in _context.Events
                        join cm in _context.ClubMembers on e.CreatedBy equals cm.MembershipId
                        where e.EventDate != null && e.EventDate >= startDate && e.EventDate < endDate
                        group e by cm.ClubId into g
                        select new
                        {
                            ClubId = g.Key,
                            EventCount = g.Count()
                        };

            var result = await query
                .Join(_context.Clubs,
                      g => g.ClubId,
                      club => club.ClubId,
                      (g, club) => new { ClubName = club.ClubName, Count = g.EventCount })
                .OrderByDescending(x => x.Count)
                .Select(x => new { item1 = x.ClubName, item2 = x.Count })
                .ToListAsync();

            return result.Cast<object>().ToList();
        }

        public async Task<List<object>> GetPostsByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from p in _context.Posts
                        join cm in _context.ClubMembers on p.CreatedBy equals cm.MembershipId
                        where p.CreatedAt != null && p.CreatedAt >= startDate && p.CreatedAt < endDate
                        group p by cm.ClubId into g
                        select new
                        {
                            ClubId = g.Key,
                            PostCount = g.Count()
                        };

            var result = await query
                .Join(_context.Clubs,
                      g => g.ClubId,
                      club => club.ClubId,
                      (g, club) => new { ClubName = club.ClubName, Count = g.PostCount })
                .OrderByDescending(x => x.Count)
                .Select(x => new { item1 = x.ClubName, item2 = x.Count })
                .ToListAsync();

            return result.Cast<object>().ToList();
        }
    }
}
