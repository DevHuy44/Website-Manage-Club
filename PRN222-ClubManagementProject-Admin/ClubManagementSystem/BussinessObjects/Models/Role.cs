using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<ClubMember> ClubMembers { get; set; } = new List<ClubMember>();
}
