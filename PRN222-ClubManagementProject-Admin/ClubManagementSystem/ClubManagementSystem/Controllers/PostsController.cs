using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using System.Security.Claims;
using Services.Interface;
using BussinessObjects.Models.Dtos;
using Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Controllers.Filter;

namespace ClubManagementSystem.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IClubMemberService _clubMemberService;
        private readonly IImageHelperService _imageService;
        private readonly SignalRSender _signalRSender;
        public PostsController(IPostService postService, IClubMemberService clubMemberService, IImageHelperService imageHelperService, SignalRSender signalRSender)
        {
            _postService = postService;
            _clubMemberService = clubMemberService;
            _imageService = imageHelperService;
            _signalRSender = signalRSender;
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (id == null)
            {
                return NotFound();
            }

            var postDetails = await _postService.GetPostDetailsAsync(id.Value);

            if (postDetails == null)
            {
                return NotFound();
            }

            ViewBag.IsMember = await _clubMemberService.IsClubMember(id.Value, userId);
            return View(postDetails);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post model, IFormFile? ImageFile, int clubId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            try
            {
                var post = await _postService.CreatePostAsync(model, ImageFile, userId, clubId);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Details", "Clubs", new { id = clubId });
            }

            return RedirectToAction("Details", "Clubs", new { id = clubId });
        }

        [ClubAdminAuthorize("Admin,Moderator")]
        public async Task<IActionResult> ApprovePost(int id)
        {
            int clubIdCheck = id;
            if (clubIdCheck == 0)
            {
                return NotFound();
            }
            var posts = await  _postService.GetPostsAsync(clubIdCheck, "Pending");
            var postViews = posts.Select(post =>
            {
                string imgBase64 = "";
                if (post.Image != null)
                {
                    imgBase64 = _imageService.ConvertToBase64(post.Image, "png");
                }

                return new PostApproveDto
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    ImageBase64 = imgBase64,
                    CreatedAt = post.CreatedAt,
                    Username = post.ClubMember.User.Username,
                    Status = post.Status
                };
            });


            return View(postViews.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CensoringPost (int postId,string status)
        {
            Notification notification;
            var post = await _postService.GetPostAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            post.Status = status;
            await _postService.UpdatePostAsync(post);
            TempData["SuccessMessage"] = status+" Successfully!";
            notification = new Notification
            {
                UserId = post.ClubMember.UserId,
                Message = "Your Post request has been " + status,
                Location = "Post Censoring"
            };
            await _signalRSender.Notify(notification, notification.UserId);
            return RedirectToAction("ApprovePost", new { id = post.ClubMember.ClubId });
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostUpdateDto postDto, IFormFile? ImageFile)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var post = await _postService.GetPostAsync(postDto.PostId);
            if (post == null || post.ClubMember.UserId != userId)
            {
                return Forbid();
            }

            if (ImageFile != null)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                postDto.ImageBase64 = Convert.ToBase64String(ms.ToArray());
            }
            try
            {
                await _postService.UpdatePostAsync(postDto);
                return RedirectToAction("Details", "Posts", new { id = post.PostId });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int postId, int clubId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var post = await _postService.GetPostAsync(postId);
            if (post == null || post.ClubMember.User.UserId != userId)
            {
                return Forbid(); 
            }

            await _postService.DeletePostAsync(postId);

            return RedirectToAction("Details", "Clubs", new { id = clubId });
        }

       

      
    }
}
