using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class ClubMemberDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ProfilePictureBase64 { get; set; } = string.Empty;

    }

    public class ClubMeberIndexDto
    {
        public int MembershipId { get; set; }
        public int ClubId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ProfilePictureBase64 { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
    }
}