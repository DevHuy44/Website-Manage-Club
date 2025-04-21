using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models
{
    public class Fee
    {
        [Key]
        public int FeeId { get; set; }

        [Required]
        public int ClubId { get; set; }

        [Required]
        public string FeeDescription { get; set; } // Mô tả đợt phí (VD: "Phí thành viên năm 2025")

        [Required]
        public string FeeType { get; set; } // Membership, Event, Penalty

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        [ForeignKey("ClubId")]
        public Club Club { get; set; }

        // Quan hệ với MembershipFee (một Fee có nhiều MembershipFee)
        public ICollection<MembershipFee> MembershipFees { get; set; }
    }
}
