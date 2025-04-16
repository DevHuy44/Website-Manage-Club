using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models;

public partial class ClubTask
{
    [Key]
    public int TaskId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [StringLength(1000)]
    public string TaskDescription { get; set; } = null!;

    //On Going, Completed, End, Cancelled
    [StringLength(50)]
    public string? Status { get; set; } = "On Going";

    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual ClubMember CreatedByNavigation { get; set; } = null!;

    [ForeignKey("EventId")]
    public virtual Event? Event { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}
