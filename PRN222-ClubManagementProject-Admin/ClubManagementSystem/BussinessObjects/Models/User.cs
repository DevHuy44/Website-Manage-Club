using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
    public string Username { get; set; } = string.Empty!;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [JsonIgnore]
    public string? Password { get; set; }
    public byte[]? ProfilePicture { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<ClubMember> ClubMembers { get; set; } = new List<ClubMember>();

    [JsonIgnore]
    public virtual ICollection<ClubRequest> ClubRequests { get; set; } = new List<ClubRequest>();

    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [JsonIgnore]
    public virtual ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();

    [JsonIgnore]
    public virtual ICollection<Connection> Connections { get; set; } = new List<Connection>();

    [JsonIgnore]
    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
}
