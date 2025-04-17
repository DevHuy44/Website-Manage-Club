using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class PostRepository : IPostRepository
    {
        private readonly FptclubsContext _context;
        public PostRepository(FptclubsContext context)
        {
            _context = context;
        }
        public async Task<Post> AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }
        public async Task<IEnumerable<Post>> GetPostsAsync(int clubId)
        {
            return await _context.Posts
                        .Include(p => p.ClubMember.User)
                        .Include(p => p.ClubMember.Club)
                        .Where(p => p.ClubMember.ClubId == clubId).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _context.Posts
                        .Include(p => p.ClubMember.User)
                        .Include(p => p.ClubMember.Club)
                        .ToListAsync();
        }

        public async Task<Post?> GetPostAsync(int postId)
        {
            return await _context.Posts
                        .Include(p => p.ClubMember.User)
                        .Include(p => p.ClubMember.Club)
                        .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<IEnumerable<Post>> GetRelatedPostsAsync(int clubId, int excludePostId, int count)
        {
            return await _context.Posts
                .Where(p => p.ClubMember.Club.ClubId == clubId && p.PostId != excludePostId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await GetPostAsync(id);
            if (post != null)
            {
                _context.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

    }
}
