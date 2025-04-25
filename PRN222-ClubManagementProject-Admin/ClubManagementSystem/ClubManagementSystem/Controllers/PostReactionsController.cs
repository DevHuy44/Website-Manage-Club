using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Services.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ClubManagementSystem.Controllers.SignalR;

namespace ClubManagementSystem.Controllers
{
    [Authorize]
    public class PostReactionsController : Controller
    {
        private readonly IPostReactionService _reactionService;
        private readonly IPostService _postService;
        private readonly SignalRSender _sender;
        public PostReactionsController(IPostReactionService postReactionService, IPostService postService, SignalRSender sender)
        {
            _reactionService = postReactionService;
            _postService = postService;
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            int likeCount = await _reactionService.ToggleReactionAsync(postId, userId);
            //_sender.NotifyPost(null, postId)

            return Json(new { success = true, likeCount });
        }



    }
}
