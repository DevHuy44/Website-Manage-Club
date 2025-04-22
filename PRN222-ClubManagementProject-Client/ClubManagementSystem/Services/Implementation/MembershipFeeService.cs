using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Services.Interface;


namespace Services.Implementation
{
    public class MembershipFeeService : IMembershipFeeService
    {
        private readonly IMembershipFeeRepository _membershipFeeRepository;
        private readonly FptclubsContext _context;

        public MembershipFeeService(IMembershipFeeRepository membershipFeeRepository, FptclubsContext context)
        {
            _membershipFeeRepository = membershipFeeRepository;
            _context = context;
        }

        public async Task<bool> CreateFeeAsync(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType)
        {
            var club = await _membershipFeeRepository.GetClubByIdAsync(clubId);
            if (club == null)
            {
                return false;
            }

            var fee = new Fee
            {
                ClubId = clubId,
                FeeDescription = feeDescription,
                FeeType = feeType,
                PaymentMethod = "VnPay",
                Amount = amount,
                DueDate = dueDate,
                CreatedAt = DateTime.Now
            };

            await _membershipFeeRepository.AddFeeAsync(fee);
            return true;
        }

        public async Task<bool> ApplyFeeToAllMembersAsync(int clubId, int feeId)
        {
            var fee = await _membershipFeeRepository.GetFeeByIdAsync(feeId);
            if (fee == null || fee.ClubId != clubId)
            {
                return false;
            }

            var clubMembers = await _membershipFeeRepository.GetClubMembersAsync(clubId);
            if (clubMembers == null || !clubMembers.Any())
            {
                return false;
            }

            foreach (var member in clubMembers)
            {
                // Kiểm tra xem thành viên đã có MembershipFee cho Fee này chưa
                var existingMembershipFee = await _context.MembershipFees
                    .FirstOrDefaultAsync(mf => mf.FeeId == feeId && mf.MemberId == member.MembershipId);

                if (existingMembershipFee != null)
                {
                    continue; // Bỏ qua nếu đã tồn tại
                }

                var membershipFee = new MembershipFee
                {
                    FeeId = feeId,
                    MemberId = member.MembershipId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                await _membershipFeeRepository.AddMembershipFeeAsync(membershipFee);

                var notification = new Notification
                {
                    UserId = member.UserId,
                    Message = $"Bạn có khoản phí mới: {fee.FeeDescription} - {fee.Amount} VND, hạn đóng {fee.DueDate:dd/MM/yyyy}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _membershipFeeRepository.AddNotificationAsync(notification);

                membershipFee.NotificationId = notification.NotificationId;
                await _membershipFeeRepository.UpdateMembershipFeeAsync(membershipFee);
            }

            return true;
        }

        public async Task<bool> CreateMembershipFeesForClubAsync(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod)
        {
            var club = await _membershipFeeRepository.GetClubByIdAsync(clubId);
            if (club == null)
            {
                return false;
            }

            var clubMembers = await _membershipFeeRepository.GetClubMembersAsync(clubId);
            if (clubMembers == null || !clubMembers.Any())
            {
                return false;
            }

            var fee = new Fee
            {
                ClubId = clubId,
                FeeDescription = feeDescription,
                FeeType = feeType,
                PaymentMethod = paymentMethod,
                Amount = amount,
                DueDate = dueDate,
                CreatedAt = DateTime.Now
            };

            fee = await _membershipFeeRepository.AddFeeAsync(fee);

            foreach (var member in clubMembers)
            {
                var membershipFee = new MembershipFee
                {
                    FeeId = fee.FeeId,
                    MemberId = member.MembershipId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                await _membershipFeeRepository.AddMembershipFeeAsync(membershipFee);

                var notification = new Notification
                {
                    UserId = member.UserId,
                    Message = $"Bạn có khoản phí mới: {feeDescription} - {amount} VND, hạn đóng {dueDate:dd/MM/yyyy}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _membershipFeeRepository.AddNotificationAsync(notification);

                membershipFee.NotificationId = notification.NotificationId;
                await _membershipFeeRepository.UpdateMembershipFeeAsync(membershipFee);
            }

            return true;
        }

        public async Task<List<Fee>> GetFeesByClubAsync(int clubId)
        {
            return await _membershipFeeRepository.GetFeesByClubAsync(clubId);
        }

        public async Task<Fee> GetFeeByIdAsync(int feeId)
        {
            return await _membershipFeeRepository.GetFeeByIdAsync(feeId);
        }

        public async Task<bool> UpdateFeeAsync(int feeId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod)
        {
            var fee = await _membershipFeeRepository.GetFeeByIdAsync(feeId);
            if (fee == null)
            {
                return false;
            }

            fee.Amount = amount;
            fee.DueDate = dueDate;
            fee.FeeDescription = feeDescription;
            fee.FeeType = feeType;
            fee.PaymentMethod = paymentMethod;

            await _membershipFeeRepository.UpdateFeeAsync(fee);

            if (fee.MembershipFees != null && fee.MembershipFees.Any())
            {
                foreach (var membershipFee in fee.MembershipFees)
                {
                    if (membershipFee.NotificationId.HasValue)
                    {
                        var notification = await _context.Notifications.FindAsync(membershipFee.NotificationId);
                        if (notification != null)
                        {
                            notification.Message = $"Cập nhật khoản phí: {feeDescription} - {amount} VND, hạn đóng {dueDate:dd/MM/yyyy}.";
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return true;
        }


        public async Task<List<MembershipFee>> GetMembershipFeesByFeeIdAsync(int feeId)
        {
            return await _membershipFeeRepository.GetMembershipFeesByFeeIdAsync(feeId);
        }

        public async Task<bool> DeleteFeeAndRelatedFeesAsync(int feeId)
        {
            var fee = await _membershipFeeRepository.GetFeeByIdAsync(feeId);
            if (fee == null)
            {
                return false;
            }

            await _membershipFeeRepository.DeleteFeeAndRelatedFeesAsync(feeId);
            return true;
        }

        public async Task<List<MembershipFee>> RemindMembersAsync(int feeId, List<int> memberIds)
        {
            var fee = await _membershipFeeRepository.GetFeeByIdAsync(feeId);
            if (fee == null)
            {
                return null; // Return an empty list if fee is not found
            }

            var membershipFees = await _context.MembershipFees
                .Where(mf => mf.FeeId == feeId && memberIds.Contains(mf.MemberId) && (mf.Status == "Pending" || mf.Status == "Overdue"))
                .Include(mf => mf.ClubMember)
                .ThenInclude(cm => cm.User)
                .ToListAsync();

            if (!membershipFees.Any())
            {
                return null; // Return an empty list if no membership fees are found
            }

            foreach (var membershipFee in membershipFees)
            {
                var notification = new Notification
                {
                    UserId = membershipFee.ClubMember.UserId,
                    Message = $"Nhắc nhở: Vui lòng thanh toán khoản phí: {fee.FeeDescription} - {fee.Amount} VND, hạn đóng {fee.DueDate:dd/MM/yyyy}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _membershipFeeRepository.AddNotificationAsync(notification);

                membershipFee.NotificationId = notification.NotificationId;
                await _membershipFeeRepository.UpdateMembershipFeeAsync(membershipFee);
            }

            return membershipFees; // Return the list of membership fees
        }

        public async Task<List<MembershipFee>> GetMembershipFeesByUserIdAsync(int userId)
        {
            return await _membershipFeeRepository.GetMembershipFeesByUserIdAsync(userId);
        }

        
    }
}
