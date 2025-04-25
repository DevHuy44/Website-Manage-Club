using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Services.Interface;
using Microsoft.AspNetCore.Authorization;
using ClubManagementSystem.Controllers.SignalR;
using System.Security.Claims;
using ClubManagementSystem.Controllers.Filter;
using Azure.Core;

namespace ClubManagementSystem.Controllers
{
    [Authorize]
    public class JoinRequestsController : Controller
    {
        private readonly IJoinRequestService _joinRequestService;
        private readonly IClubMemberService _clubMemberService;
        private readonly IRoleService _roleService;
        private readonly SignalRSender _sender;

        public JoinRequestsController(IJoinRequestService joinRequestService, IClubMemberService clubMemberService, IRoleService roleService, SignalRSender sender)
        {
            _joinRequestService = joinRequestService;
            _clubMemberService = clubMemberService;
            _roleService = roleService;
            _sender = sender;
        }

        // GET: JoinRequests
        //[ServiceFilter(typeof(ClubAuthorization))]
        [ClubAdminAuthorize("Admin")]
        public async Task<IActionResult> Index(int? id)
        {
            Console.WriteLine("Join Request");
            var requests = await _joinRequestService.GetJoinRequestsAsync(id.Value);
            ViewBag.ClubID = id;
            return View(requests);
        }

        public async Task<IActionResult> Update(int? id, string? status)
        {
            var request = await _joinRequestService.GetJoinRequestAsync(id.Value);
            var role = await _roleService.GetRoleAsync("Member");
            request.Status = status;
            if (status.Equals("Approved"))
            {
                ClubMember newMember = new ClubMember()
                {
                    ClubId = request.ClubId,
                    UserId = request.UserId,
                    RoleId = role.RoleId,
                    JoinedAt = DateTime.Now,
                    Status = true
                };
                await _clubMemberService.AddClubMemberAsync(newMember);
                Notification noti = new Notification()
                {
                    UserId = request.UserId,
                    Message = $"Your Join Request is {status} for your recent Join Club Request",
                    Location = "Join Request"
                };
                await _sender.Notify(noti, noti.UserId);
            }
            await _joinRequestService.UpdateJoinRequestAsync(request);
            return RedirectToAction("Index", new { id = request.ClubId });
        }

        public async Task<IActionResult> Create(int? clubID)
        {
            var userID = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (!(await _joinRequestService.isRequested(clubID.Value, userID)))
            {
                var newJoin = new JoinRequest()
                {
                    UserId = userID,
                    ClubId = clubID.Value,
                    CreatedAt = DateTime.Now
                };
                await _joinRequestService.AddJoinRequestAsync(newJoin);
                var adminRoleID = (await _roleService.GetRoleAsync("Admin")).RoleId;
                var admins = await _clubMemberService.GetClubMembersAsync(clubID.Value,adminRoleID);
                foreach (var admin in admins)
                {
                    Notification noti = new Notification()
                    {
                        UserId = admin.UserId,
                        Message = $"There is a new Join Request in {admin.Club.ClubName} club",
                        Location = "Join Request"
                    };
                    await _sender.Notify(noti, noti.UserId);
                }
            }
            return RedirectToAction("Index","Clubs");
        }

        //    // GET: JoinRequests/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var joinRequest = await _context.JoinRequests
        //            .Include(j => j.Club)
        //            .Include(j => j.User)
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (joinRequest == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(joinRequest);
        //    }

        //    // GET: JoinRequests/Create
        //    public IActionResult Create()
        //    {
        //        ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName");
        //        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
        //        return View();
        //    }

        //    // POST: JoinRequests/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("Id,UserId,ClubId,Status,CreatedAt")] JoinRequest joinRequest)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(joinRequest);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", joinRequest.ClubId);
        //        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", joinRequest.UserId);
        //        return View(joinRequest);
        //    }

        //    // GET: JoinRequests/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var joinRequest = await _context.JoinRequests.FindAsync(id);
        //        if (joinRequest == null)
        //        {
        //            return NotFound();
        //        }
        //        ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", joinRequest.ClubId);
        //        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", joinRequest.UserId);
        //        return View(joinRequest);
        //    }

        //    // POST: JoinRequests/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ClubId,Status,CreatedAt")] JoinRequest joinRequest)
        //    {
        //        if (id != joinRequest.Id)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(joinRequest);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!JoinRequestExists(joinRequest.Id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", joinRequest.ClubId);
        //        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", joinRequest.UserId);
        //        return View(joinRequest);
        //    }

        //    // GET: JoinRequests/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var joinRequest = await _context.JoinRequests
        //            .Include(j => j.Club)
        //            .Include(j => j.User)
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (joinRequest == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(joinRequest);
        //    }

        //    // POST: JoinRequests/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var joinRequest = await _context.JoinRequests.FindAsync(id);
        //        if (joinRequest != null)
        //        {
        //            _context.JoinRequests.Remove(joinRequest);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool JoinRequestExists(int id)
        //    {
        //        return _context.JoinRequests.Any(e => e.Id == id);
        //    }
    }
}
