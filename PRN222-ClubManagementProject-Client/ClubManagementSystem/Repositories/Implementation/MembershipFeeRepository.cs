using BussinessObjects.Models;
using Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation
{
    public class MembershipFeeRepository : IMembershipFeeRepository
    {
        private readonly FptclubsContext _context;

        public MembershipFeeRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<List<ClubMember>> GetClubMembersAsync(int clubId)
        {
            return await _context.ClubMembers
                .Where(cm => cm.ClubId == clubId && cm.Status == true)
                .Include(cm => cm.User)
                .ToListAsync();
        }

        public async Task<Club> GetClubByIdAsync(int clubId)
        {
            return await _context.Clubs.FindAsync(clubId);
        }

        public async Task<Fee> AddFeeAsync(Fee fee)
        {
            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();
            return fee;
        }

        public async Task AddMembershipFeeAsync(MembershipFee membershipFee)
        {
            _context.MembershipFees.Add(membershipFee);
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMembershipFeeAsync(MembershipFee membershipFee)
        {
            _context.MembershipFees.Update(membershipFee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Fee>> GetFeesByClubAsync(int clubId)
        {
            return await _context.Fees
                .Where(f => f.ClubId == clubId)
                .ToListAsync();
        }

        public async Task<Fee> GetFeeByIdAsync(int feeId)
        {
            return await _context.Fees
                .Include(f => f.MembershipFees)
                .ThenInclude(mf => mf.ClubMember)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(f => f.FeeId == feeId);
        }

        public async Task<List<MembershipFee>> GetMembershipFeesByFeeIdAsync(int feeId)
        {
            return await _context.MembershipFees
                .Where(mf => mf.FeeId == feeId)
                .Include(mf => mf.ClubMember)
                .ThenInclude(cm => cm.User)
                .ToListAsync();
        }

        public async Task DeleteFeeAndRelatedFeesAsync(int feeId)
        {
            var fee = await _context.Fees
                .Include(f => f.MembershipFees)
                .FirstOrDefaultAsync(f => f.FeeId == feeId);

            if (fee != null)
            {
                var notificationIds = fee.MembershipFees
                    .Where(mf => mf.NotificationId.HasValue)
                    .Select(mf => mf.NotificationId.Value)
                    .ToList();

                if (notificationIds.Any())
                {
                    var notifications = await _context.Notifications
                        .Where(n => notificationIds.Contains(n.NotificationId))
                        .ToListAsync();
                    _context.Notifications.RemoveRange(notifications);
                }

                _context.Fees.Remove(fee);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateFeeAsync(Fee fee)
        {
            _context.Fees.Update(fee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MembershipFee>> GetMembershipFeesByUserIdAsync(int userId)
        {
            return await _context.MembershipFees
                .Include(mf => mf.Fee)
                .Include(mf => mf.ClubMember)
                .ThenInclude(cm => cm.Club)
                .Where(mf => mf.ClubMember.UserId == userId)
                .ToListAsync();
        }

        public async Task<MembershipFee> GetMembershipFeeByIdAsync(int membershipFeeId, int userId)
        {
            return await _context.MembershipFees
                .Include(mf => mf.Fee)
                .Include(mf => mf.ClubMember)
                .ThenInclude(cm => cm.Club)
                .FirstOrDefaultAsync(mf => mf.MembershipFeeId == membershipFeeId && mf.ClubMember.UserId == userId);
        }

        // Phương thức mới
        public async Task<List<MembershipFee>> GetMembershipFeesByDueDateAsync(DateTime dueDate, string status)
        {
            return await _context.MembershipFees
                .Include(mf => mf.Fee)
                .Include(mf => mf.ClubMember)
                .ThenInclude(cm => cm.User)
                .Where(mf => mf.Fee.DueDate.Date == dueDate.Date && mf.Status == status)
                .ToListAsync();
        }
    }

}
