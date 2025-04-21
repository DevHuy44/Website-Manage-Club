using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Services.Interface;
using System.Security.Claims;
using BussinessObjects.Models.Dtos;
using System.ComponentModel;
using ClubManagementSystem.Controllers.SignalR;

namespace ClubManagementSystem.Controllers
{
    public class ClubRequestsController : Controller
    {
        private readonly FptclubsContext _context;
        private readonly IClubService _clubService;
        private readonly IClubRequestService _clubRequestService;
        private readonly IImageHelperService _imageService;
        private readonly SignalRSender _signalRSender;
        private readonly IClubMemberService _clubMemberService;

        public ClubRequestsController(FptclubsContext context, IClubRequestService clubRequestService, IImageHelperService imageHelperService, IClubService clubService, SignalRSender signalRSender, IClubMemberService clubMemberService)
        {
            _context = context;
            _clubRequestService = clubRequestService;
            _imageService = imageHelperService;
            _clubService = clubService;
            _signalRSender = signalRSender;
            _clubMemberService = clubMemberService;
        }

        // GET: ClubRequests
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Index()
        {
            int userId;
            if (User.IsInRole("SystemAdmin"))
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var clubRequest = await _clubRequestService.GetAllClubRequestAsync("SystemAdmin", userId);
                return View(clubRequest);
            }
            string role = User.FindFirst(ClaimTypes.Role)?.Value;
            userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _clubRequestService.GetAllClubRequestAsync(role, userId);
            return View(result);
        }

        // GET: ClubRequests/Details/5
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubRequest = _clubRequestService.GetClubRequestById(id.Value);
            if (clubRequest == null)
            {
                return NotFound();
            }
            string? logoPicture = _imageService.ConvertToBase64(clubRequest.Result.Logo, "png");
            string? coverPicture = _imageService.ConvertToBase64(clubRequest.Result.Cover, "png");
            var clubrequestView = new ClubRequestDetailDto
            {
                RequestId = clubRequest.Result.RequestId,
                UserId = clubRequest.Result.UserId,
                ClubName = clubRequest.Result.ClubName,
                Description = clubRequest.Result.Description,
                Logo = logoPicture,
                Cover = coverPicture,
                UserName = clubRequest.Result.User.Username,
                CreatedAt = clubRequest.Result.CreatedAt
            };

            return View(clubrequestView);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOrReject(int id, string status)
        {
            var clubRequest = await _clubRequestService.GetClubRequestById(id);
            Notification notification;
            if (clubRequest != null){
                if (status.Equals("Accept"))
                {
                    clubRequest.Status = "Accepted";
                    await _clubRequestService.UpdateClubRequestStatus(clubRequest);
                    Club club = new Club
                    {
                        ClubName = clubRequest.ClubName,
                        Description = clubRequest.Description,
                        Logo = clubRequest.Logo,
                        Cover = clubRequest.Cover,
                        CreatedAt = DateTime.Now,
                        Status = true
                    };
                    await _clubService.AddClubAsync(club);

                    ClubMember clubMember = new ClubMember
                    {
                        ClubId = club.ClubId,
                        UserId = clubRequest.UserId,
                        RoleId = 1,
                        JoinedAt = DateTime.Now,
                        Status = true
                    };                                                         
                    await _clubMemberService.AddClubMemberAsync(clubMember);
                    notification = new Notification
                    {
                        UserId = clubRequest.UserId,
                        Message = "Your club request has been accepted",
                        Location= "ClubRequest"
                    };
                    TempData["SuccessMessage"] = "Request Successfuly!";
                    await _signalRSender.Notify(notification, notification.UserId);
                    return RedirectToAction(nameof(Index));

                    
                }
                clubRequest.Status = "Rejected";
                await _clubRequestService.UpdateClubRequestStatus(clubRequest);
                notification = new Notification
                {
                    UserId = clubRequest.UserId,
                    Message = "Your club request has been Rejected",
                    Location = "ClubRequest"
                };
                await _signalRSender.Notify(notification, notification.UserId);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ClubRequests/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: ClubRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,UserId,ClubName,Description,Logo,Cover,Status,CreatedAt")] ClubRequest clubRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clubRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", clubRequest.UserId);
            return View(clubRequest);
        }

        // GET: ClubRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubRequest = await _context.ClubRequests.FindAsync(id);
            if (clubRequest == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", clubRequest.UserId);
            return View(clubRequest);
        }

        // POST: ClubRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,UserId,ClubName,Description,Logo,Cover,Status,CreatedAt")] ClubRequest clubRequest)
        {
            if (id != clubRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubRequestExists(clubRequest.RequestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", clubRequest.UserId);
            return View(clubRequest);
        }

        // GET: ClubRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubRequest = await _context.ClubRequests
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (clubRequest == null)
            {
                return NotFound();
            }

            return View(clubRequest);
        }

        // POST: ClubRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clubRequest = await _context.ClubRequests.FindAsync(id);
            if (clubRequest != null)
            {
                _context.ClubRequests.Remove(clubRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubRequestExists(int id)
        {
            return _context.ClubRequests.Any(e => e.RequestId == id);
        }
    }
}
