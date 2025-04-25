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
using BussinessObjects.Models.Dtos;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Controllers.Filter;

namespace ClubManagementSystem.Controllers
{
    public class ClubMembersController : Controller
    {
        private readonly FptclubsContext _context;
        private readonly IClubMemberService _clubMemberService;
        private readonly IImageHelperService _imageService;
        private readonly SignalRSender _signalRSender;

        public ClubMembersController(FptclubsContext context, IClubMemberService clubMemberService, IImageHelperService imageHelperService,SignalRSender signalRSender)
        {
            _context = context;
            _clubMemberService = clubMemberService;
            _imageService = imageHelperService;
            _signalRSender = signalRSender;
        }

        [Authorize]
        // GET: ClubMembers
        public async Task<IActionResult> Index(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var clubMembers = await _clubMemberService.GetClubMembersAsync(id);

            var clubmemberView = clubMembers.Select(member =>
            {
                string profilePictureString = "";
                if (member.User.ProfilePicture != null)
                {
                    profilePictureString = _imageService.ConvertToBase64(member.User.ProfilePicture, "png");
                }

                return new ClubMeberIndexDto
                {
                    MembershipId = member.MembershipId,
                    ClubId = id,
                    Username = member.User.Username,
                    ProfilePictureBase64 = profilePictureString,
                    Role = member.Role.RoleName,
                    JoinedAt = member.JoinedAt
                };
            });
            return View(clubmemberView.ToList());
        }

        [Authorize]
        [HttpPost]
        // Assign role for member
        public async Task<IActionResult> AssignRole(int membershipId, string role, int clubId)
        {
            if (membershipId == 0)
            {
                NotFound();
            }
            var clubMember = await _clubMemberService.GetClubMemberAsync(membershipId);
            int roleCheck = 0;
            //Check role Admin
            if (role.Equals("Admin"))
            {
                roleCheck = 1;
            }
            //Check role Morderator
            if (role.Equals("Moderator"))
            {
                roleCheck = 2;
            }
            //Check role Member
            if (role.Equals("Member"))
            {
                roleCheck = 3;
            }
            clubMember.RoleId = roleCheck;
            var (success ,message) = await _clubMemberService.UpdateClubMemberAsync(clubMember);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            return RedirectToAction("Index", new { id = clubMember.ClubId });
        }


        //Kick member
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> KickMember(int membershipId, string? reason,int clubId)
        {
            
            if (membershipId == 0)
            {
                NotFound();
            }
            Notification notification;
            var clubMember = await _clubMemberService.GetClubMemberAsync(membershipId);
            if(clubMember.Role.RoleName.Equals("Admin"))
            {
                TempData["ErrorMessage"] = "Can not kick club admin!";
                return RedirectToAction("Index", new { id = clubMember.ClubId });
            }
            clubMember.Status = false;
            var (success, message) = await _clubMemberService.UpdateClubMemberAsync(clubMember);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            if (success)
            {
                if (String.IsNullOrEmpty(reason))
                {
                    reason = "";
                }
                notification = new Notification
                {
                    UserId = clubMember.UserId,
                    Message = reason ?? "Nothing",
                    Location = "you have been kick out by club admin"
                };

                await _signalRSender.Notify(notification, notification.UserId);
            }
            return RedirectToAction("Index", new { id = clubMember.ClubId });
        }                     
    }
}
