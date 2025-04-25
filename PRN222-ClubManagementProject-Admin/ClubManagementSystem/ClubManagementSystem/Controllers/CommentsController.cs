using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Services.Interface;
using BussinessObjects.Models.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ClubManagementSystem.Controllers.SignalR;

namespace ClubManagementSystem.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IImageHelperService _imageService;
        private readonly SignalRSender _sender;

        public CommentsController(ICommentService commentService, SignalRSender sender, IImageHelperService imageService)
        {
            _commentService = commentService;
            _sender = sender;
            _imageService = imageService;
        }

 

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentDto commentDto)
        {
            //if (string.IsNullOrWhiteSpace(commentDto.CommentText))
            //    return BadRequest("Comment cannot be empty.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                PostId = commentDto.PostId,
                UserId = userId,
                CommentText = commentDto.CommentText,
                CreatedAt = DateTime.Now
            };
            await _commentService.AddCommentAsync(comment);

            var newComment = await _commentService.GetCommentDtoAsync(comment.CommentId);
            await _sender.NotifyPost(newComment, null);
            return RedirectToAction("Details", "Posts", new { id = commentDto.PostId });
        }



        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(CommentDto commentDto)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var success = await _commentService.UpdateCommentAsync(commentDto.CommentId, commentDto.CommentText, userId);
            if (!success)
            {
                TempData["Error"] = "Failed to update comment.";
                return RedirectToAction("Details", "Posts", new { id = commentDto.PostId });
            }

            var newComment = await _commentService.GetCommentDtoAsync(commentDto.CommentId);
            await _sender.NotifyPost(newComment, null);
            return RedirectToAction("Details", "Posts", new { id = commentDto.PostId });
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var comment = await _commentService.GetCommentAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            int postId = comment.PostId;

            var result = await _commentService.DeleteCommentAsync(id, userId);

            if (!result)
            {
                TempData["Error"] = "Failed to delete comment.";
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            await _sender.NotifyDeletePost(comment, null);

            return RedirectToAction("Details", "Posts", new { id = postId });
        }

       
    }
}
