using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IPostReactionRepository
    {
        Task<PostReaction?> GetReactionAsync(int postId, int userId);
        Task<int> GetLikeCountAsync(int postId);
        Task AddReactionAsync(PostReaction reaction);
        Task RemoveReactionAsync(PostReaction reaction);
        Task UpdateReactionAsync(PostReaction reaction);
       
    }
}
