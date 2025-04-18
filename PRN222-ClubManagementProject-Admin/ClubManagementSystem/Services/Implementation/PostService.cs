using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IClubMemberService _clubMemberService;
        private readonly ICommentService _commentService;
        private readonly IImageHelperService _imageHelperService;
        private readonly IPostReactionService _postReactionService;
        public PostService(IPostRepository postRepository, IClubMemberService clubMemberService, ICommentService commentService , IImageHelperService imageHelperService, IPostReactionService postReactionService)
        {
            _postRepository = postRepository;
            _clubMemberService = clubMemberService;
            _commentService = commentService;
            _imageHelperService = imageHelperService;
            _postReactionService = postReactionService;
            _postReactionService = postReactionService;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int clubId, string status)
        {
            var post = await _postRepository.GetPostsAsync(clubId);
            return post.Where(p => p.Status == status);
        }  
        public async Task<IEnumerable<Post>> GetPostsAsync(string status)
        {
            var post = await _postRepository.GetPostsAsync();
            return post.Where(p => p.Status == status);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int clubId)
        {
            return await _postRepository.GetPostsAsync(clubId);
        }
        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _postRepository.GetPostsAsync();
        }
        public async Task<Post> CreatePostAsync(Post model, IFormFile? imageFile, int userId, int clubId)
        {
            var clubMember = await _clubMemberService.GetClubMemberAsync(clubId, userId);
            if (clubMember == null)
            {
                throw new UnauthorizedAccessException("User is not a member of the club.");
            }

            var post = new Post
            {
                CreatedBy = clubMember.MembershipId,
                Title = model.Title,
                Content = model.Content,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                post.Image = ms.ToArray();
            }

            return await _postRepository.AddAsync(post);
        }

        public async Task<PostDetailsDto?> GetPostDetailsAsync(int postId)
        {
            var post = await _postRepository.GetPostAsync(postId);

            if (post == null)
            {
                return null; 
            }
            var relatedPosts = await _postRepository.GetRelatedPostsAsync(post.ClubMember.ClubId, post.PostId, 3);

            var comments = await _commentService.GetCommentsByPostIdAsync(postId);

            var viewModel = new PostDetailsDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                ImageBase64 = _imageHelperService.ConvertToBase64(post.Image, "png"),
                CreatedAt = post.CreatedAt,
                Status = post.Status,

                User = new UserDto
                {
                    UserId = post.ClubMember.UserId,
                    Username = post.ClubMember.User.Username,
                },
                Club = new ClubDto
                {
                    ClubId = post.ClubMember.ClubId,
                    ClubName = post.ClubMember.Club.ClubName
                },
                Comments = post.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    PostId = c.PostId,
                    CommentText = c.CommentText,
                    CreatedAt = c.CreatedAt,
                    User = new UserDto
                    {
                        UserId = c.User.UserId,  
                        Username = c.User.Username,
                        Email = c.User.Email,
                        ProfilePictureBase64 = _imageHelperService.ConvertToBase64(c.User.ProfilePicture, "png"),
                    }
                }).ToList(),
                LikeCount = await _postReactionService.GetLikeCountAsync(post.PostId)
            };

            if (viewModel == null)
            {
                return null;
            }

            return viewModel;

        }

        public async Task UpdatePostAsync(Post post)
        {
            await _postRepository.UpdatePostAsync(post);
        }

        public async Task UpdatePostAsync(PostUpdateDto postDto)
        {
            var post = await _postRepository.GetPostAsync(postDto.PostId);
            if (post == null) 
            {
                throw new DbUpdateConcurrencyException();
            }

            post.Title = postDto.Title;
            post.Content = postDto.Content;

            if (postDto.ImageBase64 != null)
            {
                post.Image = _imageHelperService.ConvertToByte(postDto.ImageBase64);
            }

            await _postRepository.UpdatePostAsync(post);
        }
        public async Task<Post> GetPostAsync(int postId)
        {
            return await _postRepository.GetPostAsync(postId);
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _postRepository.GetPostAsync(id);
            post.Status = "Deleted";
            await _postRepository.UpdatePostAsync(post);

        }

    }
}
