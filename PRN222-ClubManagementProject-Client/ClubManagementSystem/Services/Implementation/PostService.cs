using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IPostReactionRepository _reactionRepository;

        public PostReactionService(IPostReactionRepository reactionRepo)
        {
            _reactionRepository = reactionRepo;
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            return await _reactionRepository.GetLikeCountAsync(postId);
        }
        public async Task<int> ToggleReactionAsync(int postId, int userId)
        {
            var reaction = await _reactionRepository.GetReactionAsync(postId, userId);

            if (reaction == null)
            {
                reaction = new PostReaction
                {
                    PostId = postId,
                    UserId = userId,
                    IsLiked = true
                };
                await _reactionRepository.AddReactionAsync(reaction);
            }
            else
            {
                reaction.IsLiked = !reaction.IsLiked; 
                await _reactionRepository.UpdateReactionAsync(reaction);
            }

            return await GetLikeCountAsync(postId);
        }


    }
}