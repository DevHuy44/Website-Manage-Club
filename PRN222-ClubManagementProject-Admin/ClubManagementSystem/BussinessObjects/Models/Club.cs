using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BussinessObjects.Models;

public partial class Club
{
    [Key]
    public int ClubId { get; set; }

    [Required]
    [StringLength(100)]
    public string ClubName { get; set; } = null!;
    public byte[]? Logo { get; set; }
    public byte[]? Cover { get; set; }

    public bool Status { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ClubMember> ClubMembers { get; set; } = new List<ClubMember>();
    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
}
