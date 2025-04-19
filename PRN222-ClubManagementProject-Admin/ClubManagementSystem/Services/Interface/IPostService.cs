using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(Post model, IFormFile? imageFile, int userId, int clubId);
        Task<IEnumerable<Post>> GetPostsAsync(int clubId);
        Task<IEnumerable<Post>> GetPostsAsync(int clubId, string status);
        Task<IEnumerable<Post>> GetPostsAsync(string status);
        Task<IEnumerable<Post>> GetPostsAsync();
        Task<PostDetailsDto?> GetPostDetailsAsync(int postId);
        Task UpdatePostAsync(PostUpdateDto postDto);
        Task UpdatePostAsync(Post post);
        Task<Post> GetPostAsync(int postId);
        Task DeletePostAsync(int postId);

    }
}
