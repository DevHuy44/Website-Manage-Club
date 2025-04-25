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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace ClubManagementSystem.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IClubMemberService _clubMemberService;
        private readonly IImageHelperService _imageService;
        private readonly SignalRSender _signalRSender;
        private readonly Cloudinary _cloudinary;
        public PostsController(IPostService postService, 
            IClubMemberService clubMemberService, 
            IImageHelperService imageHelperService, 
            SignalRSender signalRSender,  Cloudinary cloudinary)
        {
            _postService = postService;
            _clubMemberService = clubMemberService;
            _imageService = imageHelperService;
            _signalRSender = signalRSender;
            _cloudinary = cloudinary;
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

        //Check if the user is a member of the club
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

            string imageUrl = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only .jpg, .jpeg, and .png files are allowed for post images.";
                    return RedirectToAction("Details", "Clubs", new { id = clubId });
                }

                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                    Folder = "post_images",
                    PublicId = $"post_{clubId}_{userId}_{DateTime.Now.Ticks}",
                    //Transformation = new Transformation().Width(800).Height(600).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    TempData["ErrorMessage"] = "Image upload failed. Please try again.";
                    return RedirectToAction("Details", "Clubs", new { id = clubId });
                }

                imageUrl = uploadResult.SecureUrl.ToString();
            }

            try
            {
                var post = await _postService.CreatePostAsync(model, imageUrl, userId, clubId);
                TempData["SuccessMessage"] = "Post created successfully!";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "You are not authorized to create a post in this club.";
                return RedirectToAction("Details", "Clubs", new { id = clubId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
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
               
                return new PostApproveDto
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    ImageBase64 = post.Image_Url,
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
            string imageUrl = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only .jpg, .jpeg, and .png files are allowed for post images.";
                    return RedirectToAction("Details", "Posts", new { id = post.PostId });
                }

                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                    Folder = "post_images",
                    PublicId = $"post_{postDto.PostId}_{userId}_{DateTime.Now.Ticks}",
                    //Transformation = new Transformation().Width(800).Height(600).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    TempData["ErrorMessage"] = "Image upload failed. Please try again.";
                    return RedirectToAction("Details", "Posts", new { id = post.PostId });
                }

                imageUrl = uploadResult.SecureUrl.ToString();
            }
            
                postDto.ImageBase64 = imageUrl;
            
            try
            {
                await _postService.UpdatePostAsync(postDto);
                TempData["SuccessMessage"] = "Post edit successfully!";
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
            //if (post == null || post.ClubMember.User.UserId != userId)
            //{
            //    return Forbid();
            //}

            await _postService.DeletePostAsync(postId);

            return RedirectToAction("Details", "Clubs", new { id = clubId });
        }

       

      
    }
}
