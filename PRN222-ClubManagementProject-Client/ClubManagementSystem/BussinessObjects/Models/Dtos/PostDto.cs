using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageBase64 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public int CreatedBy { get; set; }
    }


    public class PostApproveDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageBase64 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public string Username { get; set; }
    }
    public class PostDetailsDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageBase64 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public UserDto User { get; set; } = new();
        public ClubDto Club { get; set; } = new();
        public List<RelatedPostDto> RelatedPosts { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
        public int LikeCount { get; set; }

    }

    public class RelatedPostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string ImageBase64 { get; set; }
    }

    public class PostUpdateDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageBase64 { get; set; }
    }

}
