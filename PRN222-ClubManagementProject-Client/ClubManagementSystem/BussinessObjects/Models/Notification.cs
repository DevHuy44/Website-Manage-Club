using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models;

public partial class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; } = false;

    public string? Location { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
