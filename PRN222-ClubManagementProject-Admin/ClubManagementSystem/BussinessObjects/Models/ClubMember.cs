using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class ClubMember
{
    [Key]
    public int MembershipId { get; set; }

    [Required]
    public int ClubId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoleId { get; set; }

    public bool Status { get; set; }  

    [DataType(DataType.DateTime)]
    public DateTime? JoinedAt { get; set; }

    [ForeignKey("ClubId")]
    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<ClubTask> Tasks { get; set; } = new List<ClubTask>();

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
