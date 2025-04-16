using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class Event
{
    [Key]
    public int EventId { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    [StringLength(200)]
    public string EventTitle { get; set; } = null!;

    [StringLength(1000)]
    public string? EventDescription { get; set; }

    public string? Status { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EventDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CreatedBy")]
    public virtual ClubMember CreatedByNavigation { get; set; } = null!;
    public virtual List<ClubTask>? Tasks { get; set; }
}
