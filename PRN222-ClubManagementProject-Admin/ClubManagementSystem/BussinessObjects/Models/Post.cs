using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class Post
{
    [Key]
    public int PostId { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(MAX)")]
    public string Content { get; set; } = null!;
    public byte[]? Image { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [StringLength(50)]
    public string? Status { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual ClubMember ClubMember { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
}
