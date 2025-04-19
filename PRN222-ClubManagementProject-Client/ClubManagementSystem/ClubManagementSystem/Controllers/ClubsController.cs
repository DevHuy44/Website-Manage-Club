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
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Repositories.Interface;
using Services.Implementation;
using Azure.Core;
using Microsoft.Extensions.Hosting;
using ClubManagementSystem.Controllers.Filter;

namespace ClubManagementSystem.Controllers
{
    public class ClubsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IClubRequestService _clubRequestService;
        private readonly IClubMemberService _clubMemberService;
        private readonly IClubService _clubService;
        private readonly IJoinRequestService _joinRequestService;
        private readonly IImageHelperService _imageService;
        private readonly IEventService _eventService;
        private readonly FptclubsContext _context;

        private readonly IPostService _postService;
        public ClubsController(FptclubsContext context, IClubRequestService clubRequestService, IAccountService accountService, IClubService clubService, IPostService postService, IJoinRequestService joinRequestService, IClubMemberService clubMemberService, IImageHelperService imageHelperService, IEventService eventService)
        {
            _context = context;
            _clubRequestService = clubRequestService;
            _accountService = accountService;
            _clubService = clubService;
            _joinRequestService = joinRequestService;
            _clubMemberService = clubMemberService;
            _postService = postService;
            _imageService = imageHelperService;
            _eventService = eventService;
        }

        // GET: Clubs
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            var clubs = await _clubService.GetAllClubsAsync();
            //Check if user search
            if (!String.IsNullOrEmpty(searchString))
            {
                clubs = clubs.Where(s => s.ClubName.Contains(searchString)).ToList();
            }
            //paging
            var pagedClubs = clubs
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            int totalRecords = await _context.Clubs.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.SearchString = searchString;
            return View(pagedClubs);
        }

        [Authorize]
        public async Task<IActionResult> YourClubs(string? searchString, int? pageNumber)
        {
            int pageSize = 20;
            int currentPage = pageNumber ?? 1;
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var clubsMember = await _clubMemberService.GetClubMemberByUserId(userId);
            var clubs = clubsMember.Count() > 0 ? clubsMember.Select(m => m.Club).ToList() : new List<Club>();
            //Check if user search
            if (!String.IsNullOrEmpty(searchString))
            {
                clubs = clubs.Where(s => s.ClubName.Contains(searchString)).ToList();
            }
            var pagedClubs = clubs
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            int totalRecords = await _context.Clubs.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.SearchString = searchString;
            return View(pagedClubs);
        }

        public async Task<IActionResult> Request(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var requests = _joinRequestService.GetJoinRequestsAsync(id.Value);
            return View(requests);
        }

        // GET: Clubs/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id, int? pageNumber)
        {
            int postSize = 5;
            int currentPage = pageNumber ?? 1;
            int userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _clubService.GetClubDetailsAsync(id.Value, currentPage, postSize);
            viewModel.IncomingEvents = (List<Event>)(await _eventService.GetIncomingEvent(viewModel.ClubId));
            viewModel.OngoingEvent = await _eventService.GetOnGoingEvent(viewModel.ClubId);

            if (viewModel == null)
            {
                return NotFound();
            }

            ViewBag.IsMember = await _clubMemberService.IsClubMember(id.Value, userID);

            return View(viewModel);
        }

        [Authorize]
        [ClubAdminAuthorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteClub(int? id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var club = await _clubService.GetClubByClubIdAsync(id.Value);
            club.Status = false;
            var (success, message) = await _clubService.DeleteClub(club);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            return RedirectToAction("Index", "Clubs");
        }

        [Authorize]
        // GET: Clubs/Create
        public IActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile logoPicture, string clubName, string description, IFormFile coverPicture)
        {
            if ((logoPicture != null && logoPicture.Length > 0) || (coverPicture != null && coverPicture.Length > 0))
            {
                var club = await _clubService.CheckClubName(clubName);

                //check if club name already exists
                if (club != null)
                {
                    TempData["ErrorMessage"] = "Club name already exists!";
                    return View();
                }
                // Convert image to byte array
                byte[] logoPictureBytes;
                byte[] coverPictureBytes;
                using (var logomemoryStream = new MemoryStream())
                {
                    await logoPicture.CopyToAsync(logomemoryStream);
                    logoPictureBytes = logomemoryStream.ToArray();
                }
                using (var covermemoryStream = new MemoryStream())
                {
                    await coverPicture.CopyToAsync(covermemoryStream);
                    coverPictureBytes = covermemoryStream.ToArray();
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    var clubRequest = new ClubRequest
                    {
                        ClubName = clubName,
                        Description = description,
                        Logo = logoPictureBytes,
                        Cover = coverPictureBytes,
                        CreatedAt = DateTime.Now,
                        UserId = int.Parse(userId),
                        Status = "Pending"
                    };
                    await _clubRequestService.AddClubRequestAsync(clubRequest);
                    TempData["SuccessMessage"] = "Club created successfully!";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["ErrorMessage"] = "Please fill logo picture and cover picture!";
            return View();
        }

        [Authorize]
        [ClubAdminAuthorize("Admin,Moderator")]
        public async Task<IActionResult> Edit(int Id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                NotFound();
            }
            //if (User.IsInRole("SystemAdmin"))
            //{

            //}
            ////var clubmember = await _clubMemberService.GetClubMemberByUserId(int.Parse(userId));
            var club = await _clubService.GetClubByClubIdAsync(Id);
            var logoPicture = _imageService.ConvertToBase64(club.Logo, "png");
            var coverPicture = _imageService.ConvertToBase64(club.Cover, "png");
            var clubView = new ClubEditViewDto
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                Description = club.Description,
                Logo = logoPicture,
                Cover = coverPicture
            };
            return View(clubView);
        }

        [Authorize]
        [ClubAdminAuthorize("Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> Edit(ClubEditViewDto clubViewEditDto)
        {
            var clubEditDto = new ClubEditDto
            {
                ClubId = clubViewEditDto.ClubId,
                ClubName = clubViewEditDto.ClubName,
                Description = clubViewEditDto.Description,
            };
            var (success, message) = await _clubService.UpdateClubAsync(clubEditDto);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            var club = await _clubService.GetClubByClubIdAsync(clubEditDto.ClubId);
            var logoPicture = _imageService.ConvertToBase64(club.Logo, "png");
            var coverPicture = _imageService.ConvertToBase64(club.Cover, "png");
            var clubView = new ClubEditViewDto
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                Description = club.Description,
                Logo = logoPicture,
                Cover = coverPicture
            };
            return View(clubView);
        }

        [Authorize]
        [ClubAdminAuthorize("Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogoPicture(IFormFile logoPicture,int? id)
        {
            if (logoPicture != null && logoPicture.Length > 0)
            {
                // Convert image to byte array
                byte[] logoPictureBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await logoPicture.CopyToAsync(memoryStream);
                    logoPictureBytes = memoryStream.ToArray();
                }

                var club = await _clubService.GetClubByClubIdAsync(id.Value);

                if (club != null)
                {
                    var clubEditDto = new ClubEditDto
                    {
                        ClubId = id.Value,
                        Logo = logoPictureBytes
                    };
                    var (success, message) = await _clubService.UpdateClubAsync(clubEditDto);
                    TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            ModelState.AddModelError("logoPicture", "No file selected.");
            return RedirectToAction("Edit", new { id = id });
        }

        [Authorize]
        [ClubAdminAuthorize("Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCoverPicture(IFormFile coverPicture, int? clubId)
        {
            if (coverPicture != null && coverPicture.Length > 0)
            {
                // Convert image to byte array
                byte[] logoPictureBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await coverPicture.CopyToAsync(memoryStream);
                    logoPictureBytes = memoryStream.ToArray();
                }

                var club = await _clubService.GetClubByClubIdAsync(clubId.Value);

                if (club != null)
                {
                    var clubEditDto = new ClubEditDto
                    {
                        ClubId = clubId.Value,
                        Cover = logoPictureBytes
                    };
                    var (success, message) = await _clubService.UpdateClubAsync(clubEditDto);
                    TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
                    return RedirectToAction("Edit", new { id = clubId });
                }
            }
            ModelState.AddModelError("CoverPicture", "No file selected.");
            return RedirectToAction("Edit", new { id = clubId });
        }      
    }
}
