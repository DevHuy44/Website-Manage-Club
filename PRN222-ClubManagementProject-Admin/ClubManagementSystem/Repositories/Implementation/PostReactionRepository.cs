using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class PostReactionRepository : IPostReactionRepository
    {
        private readonly FptclubsContext _context;

        public PostReactionRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<PostReaction?> GetReactionAsync(int postId, int userId)
        {
            return await _context.Reactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            return await _context.Reactions.CountAsync(r => r.PostId == postId && r.IsLiked);
        }

        public async Task AddReactionAsync(PostReaction reaction)
        {
            await _context.Reactions.AddAsync(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveReactionAsync(PostReaction reaction)
        {
            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReactionAsync(PostReaction reaction)
        {
            _context.Reactions.Update(reaction);
            await _context.SaveChangesAsync();
        }
       

    }
}
