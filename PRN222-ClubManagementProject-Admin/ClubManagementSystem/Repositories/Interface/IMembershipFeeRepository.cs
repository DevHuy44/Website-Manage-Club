using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IMembershipFeeRepository
    {
        Task<List<ClubMember>> GetClubMembersAsync(int clubId);
        Task<Club> GetClubByIdAsync(int clubId);
        Task<Fee> AddFeeAsync(Fee fee);
        Task AddMembershipFeeAsync(MembershipFee membershipFee);
        Task AddNotificationAsync(Notification notification);
        Task UpdateMembershipFeeAsync(MembershipFee membershipFee);
        Task<List<Fee>> GetFeesByClubAsync(int clubId);
        Task<Fee> GetFeeByIdAsync(int feeId);
        Task<List<MembershipFee>> GetMembershipFeesByFeeIdAsync(int feeId);
        Task DeleteFeeAndRelatedFeesAsync(int feeId);
        Task UpdateFeeAsync(Fee fee);
        Task<List<MembershipFee>> GetMembershipFeesByUserIdAsync(int userId);
        Task<MembershipFee> GetMembershipFeeByIdAsync(int membershipFeeId, int userId);
        // Phương thức mới
        Task<List<MembershipFee>> GetMembershipFeesByDueDateAsync(DateTime dueDate, string status);
    }
}
