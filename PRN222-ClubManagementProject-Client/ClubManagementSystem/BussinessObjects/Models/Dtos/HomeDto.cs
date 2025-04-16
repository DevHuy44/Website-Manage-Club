using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class HomeDto
    {
        public IEnumerable<Event> Events { get; set; }
        public List<PostDetailsDto> Posts { get; set; } = new();
        public List<ClubDetailsViewDto> Clubs { get; set; } = new();
    }

}
