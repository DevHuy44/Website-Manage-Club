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
using Microsoft.Extensions.Logging;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Controllers.Filter;

namespace ClubManagementSystem.Controllers
{
    [Authorize]
    public class ClubTasksController : Controller
    {
        //delet after
        private readonly FptclubsContext _context;
        private readonly IClubTaskService _taskService;
        private readonly IEventService _eventService;
        private readonly IClubMemberService _memberService;
        private readonly ITaskAssignmentService _assignervice;
        private readonly SignalRSender _sender;

        public ClubTasksController(FptclubsContext context, IClubTaskService taskService, IClubMemberService memberService, IEventService eventService, ITaskAssignmentService assignervice, 
            SignalRSender sender)
        {
            _context = context;
            _taskService = taskService;
            _eventService = eventService;
            _memberService = memberService;
            _assignervice = assignervice;
            _sender = sender;
        }

        //GET: ClubTasks
        public async Task<IActionResult> Index()
        {
            int userID = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tasks = await _taskService.GetPersonalClubTasksAsync(userID);
            return View(tasks);
        }

        // GET: ClubTasks/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id, string? error)
        {
            var task = await _taskService.GetClubTask(id.Value);
            var members = await _memberService.GetClubMembersAsync(task.CreatedByNavigation.ClubId);
            ViewBag.Error = error;
            ViewBag.Members = new SelectList(members, "MembershipId", "User.Username");
            return View(task);
        }

        [ClubAdminAuthorize("Admin,Moderator")]
        // GET: ClubTasks/Create
        public IActionResult Create(int eventID,int id)
        {
            ViewBag.EventID = eventID;
            return View();
        }

        // POST: ClubTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,TaskDescription,DueDate")] ClubTask clubTask)
        {
            var userID = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int clubID = (await _eventService.GetEventAsync(clubTask.EventId)).CreatedByNavigation.ClubId;
            var memberID = (await _memberService.GetClubMemberAsync(clubID, userID)).MembershipId;

            clubTask.CreatedBy = memberID;
            await _taskService.AddClubTaskAsync(clubTask);
            
            return RedirectToAction("Tasks", "Events", new { EventId = clubTask.EventId, id = clubID });
        }

        [ClubAdminAuthorize("Admin,Moderator")]
        // GET: ClubTasks/Edit/5
        public async Task<IActionResult> Edit(int? taskId,int clubId)
        {
            var task = await _taskService.GetClubTask(taskId.Value);
            return View(task);
        }

        // POST: ClubTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskDescription,DueDate")] ClubTask clubTask)
        {
            var task = await _taskService.GetClubTask(id);
            task.TaskDescription = clubTask.TaskDescription;
            task.DueDate = clubTask.DueDate;
            await _taskService.UpdateClubTaskAsync(task);

            var assigns = await _assignervice.GetTaskAssignmentsAsync(id);
            foreach (var assign in assigns)
            {
                Notification noti = new Notification()
                {
                    Message = "Your assigned Task have been updated",
                    Location = "Tasks",
                    UserId = assign.Membership.UserId
                };
                await _sender.Notify(noti, noti.UserId);
            }

            return RedirectToAction("Details", "ClubTasks", new { id = task.TaskId });
        }

        // GET: ClubTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var clubTask = await _taskService.GetClubTask(id.Value);
            return View(clubTask);
        }

        // POST: ClubTasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isAssigned = await _taskService.IsAssigned(id);
            var eventID = (await _taskService.GetClubTask(id)).EventId;
            if (isAssigned)
            {
                var error = "Task is Assigned, please Unassigned all to delete";
                return RedirectToAction("Tasks", "Events", new { id = eventID, error = error });
            }
            var task = await _taskService.GetClubTask(id);
            task.Status = "Cancelled";
            await _taskService.UpdateClubTaskAsync(task);

            return RedirectToAction("Tasks", "Events", new { id = eventID });
        }
    }
}
