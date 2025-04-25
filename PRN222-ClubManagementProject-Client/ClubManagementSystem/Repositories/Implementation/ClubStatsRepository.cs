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

        public async Task<List<(string ClubName, int MemberCount)>> GetMembersByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from cm in _context.ClubMembers
                        join club in _context.Clubs on cm.ClubId equals club.ClubId
                        where cm.JoinedAt != null && cm.JoinedAt >= startDate && cm.JoinedAt < endDate
                        group cm by new { club.ClubId, club.ClubName } into g
                        select new ValueTuple<string, int>(g.Key.ClubName, g.Count());

            return await query
                .OrderByDescending(x => x.Item2)
                .ToListAsync();
        }

        public async Task<List<(string ClubName, int EventCount)>> GetEventsByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from e in _context.Events
                        join cm in _context.ClubMembers on e.CreatedBy equals cm.MembershipId
                        join club in _context.Clubs on cm.ClubId equals club.ClubId
                        where e.EventDate != null && e.EventDate >= startDate && e.EventDate < endDate
                        group e by new { club.ClubId, club.ClubName } into g
                        select new ValueTuple<string, int>(g.Key.ClubName, g.Count());

            return await query
                .OrderByDescending(x => x.Item2)
                .ToListAsync();
        }

        public async Task<List<(string ClubName, int PostCount)>> GetPostsByClubAsync(DateTime startDate, DateTime endDate)
        {
            var query = from p in _context.Posts
                        join cm in _context.ClubMembers on p.CreatedBy equals cm.MembershipId
                        join club in _context.Clubs on cm.ClubId equals club.ClubId
                        where p.CreatedAt != null && p.CreatedAt >= startDate && p.CreatedAt < endDate
                        group p by new { club.ClubId, club.ClubName } into g
                        select new ValueTuple<string, int>(g.Key.ClubName, g.Count());

            return await query
                .OrderByDescending(x => x.Item2)
                .ToListAsync();
        }
    }
}
