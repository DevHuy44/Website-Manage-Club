using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMembershipFeeService
    {
        Task<bool> CreateFeeAsync(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType); // Chỉ tạo Fee
        Task<bool> ApplyFeeToAllMembersAsync(int clubId, int feeId); // Áp dụng Fee cho tất cả thành viên
        Task<bool> CreateMembershipFeesForClubAsync(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod);
        Task<List<Fee>> GetFeesByClubAsync(int clubId);
        Task<Fee> GetFeeByIdAsync(int feeId);
        Task<bool> UpdateFeeAsync(int feeId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod);
        Task<List<MembershipFee>> GetMembershipFeesByFeeIdAsync(int feeId);
        Task<bool> DeleteFeeAndRelatedFeesAsync(int feeId);
        Task<List<MembershipFee>> RemindMembersAsync(int feeId, List<int> memberIds); // Gửi nhắc nhở
        Task<List<MembershipFee>> GetMembershipFeesByUserIdAsync(int userId);
        
    }
}
