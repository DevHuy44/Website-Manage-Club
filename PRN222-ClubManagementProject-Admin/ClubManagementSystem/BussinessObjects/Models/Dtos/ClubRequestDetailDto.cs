using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class ClubRequestDetailDto
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string ClubName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string? Cover { get; set; }
        public string? UserName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }
    }
}
