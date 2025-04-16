using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class ClubDetailsViewDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string LogoBase64 { get; set; } = string.Empty;
        public string CoverBase64 { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Event? OngoingEvent { get; set; }
        public List<Event> IncomingEvents { get; set; } = new List<Event>();
        public List<ClubMemberDto> ClubMembers { get; set; } = new List<ClubMemberDto>();
        public List<PostDetailsDto> Posts { get; set; } = new List<PostDetailsDto>();

        public int TotalPosts { get; set; }
        public int PostNumber { get; set; }
        public int PostSize { get; set; }

    }

    public class ClubDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public byte[]? Logo { get; set; }
        public byte[]? Cover { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<ClubMemberDto> ClubMembers { get; set; } = new List<ClubMemberDto>();

    }

}
