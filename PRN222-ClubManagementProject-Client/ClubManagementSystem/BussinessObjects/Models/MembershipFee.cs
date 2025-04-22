using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models
{
    public class MembershipFee
    {
        [Key]
        public int MembershipFeeId { get; set; } // Đổi tên FeeId thành MembershipFeeId để tránh nhầm lẫn

        [Required]
        public int FeeId { get; set; } // Liên kết với bảng Fee

        [Required]
        public int MemberId { get; set; } // Liên kết với ClubMember

        [Required]
        public string Status { get; set; } // Pending, Paid, Overdue

        public DateTime? PaidAt { get; set; }

        public string? TransactionId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("FeeId")]
        public Fee Fee { get; set; }

        [ForeignKey("MemberId")]
        public ClubMember ClubMember { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }

        public int? NotificationId { get; set; }
    }
}
