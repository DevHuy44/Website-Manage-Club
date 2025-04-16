using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class ClubRequest
{
    [Key]
    public int RequestId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string ClubName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
    public byte[]? Logo { get; set; }
    public byte[]? Cover { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
