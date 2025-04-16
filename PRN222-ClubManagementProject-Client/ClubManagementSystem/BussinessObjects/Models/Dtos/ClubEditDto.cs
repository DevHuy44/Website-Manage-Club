using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class ClubEditDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public byte[]? Logo { get; set; }
        public byte[]? Cover { get; set; }
        public string? Description { get; set; }
    }

    public class ClubEditViewDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? Cover { get; set; }
        public string? Description { get; set; }
    }
}
