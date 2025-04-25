using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<Comment?> GetCommentAsync(int id);
        Task<CommentDto?> GetCommentDtoAsync(int id);
        Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> UpdateCommentAsync(int commentId, string newText, int userId);
        Task<bool> DeleteCommentAsync(int commentId, int userId);

    }
}
