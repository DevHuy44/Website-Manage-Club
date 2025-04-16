using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models
{
    public class Connection
    {
        public int Id { get; set; }
        [Required]
        public string? ConnectionId { get; set; }
        [Required]
        public int? UserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? connectAt { get; set; } = DateTime.Now;

        //navigation property
        public virtual User? User { get; set; }
    }
}
