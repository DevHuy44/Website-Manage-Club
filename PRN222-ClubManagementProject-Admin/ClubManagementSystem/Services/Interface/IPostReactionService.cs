using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPostReactionService
    {
        Task<int> GetLikeCountAsync(int postId);
        Task<int> ToggleReactionAsync(int postId, int userId);
    }
}
