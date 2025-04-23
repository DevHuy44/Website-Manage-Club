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
using ClubManagementSystem.Controllers.Filter;
using ClubManagementSystem.Controllers.SignalR;

namespace ClubManagementSystem.Controllers
{
    public class TaskAssignmentsController : Controller
    {
        private readonly FptclubsContext _context;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly IClubMemberService _memberService;
        private readonly SignalRSender _sender;

        public TaskAssignmentsController(ITaskAssignmentService taskAssignmentService, SignalRSender sender, IClubMemberService memberService)
        {
            _taskAssignmentService = taskAssignmentService;
            _sender = sender;
            _memberService = memberService;
        }

        //[ClubAdminAuthorize("Admin,Moderator")]
        public async Task<IActionResult> Assign(int taskID, int memberID,int id)
        {
            var isAssigned = await _taskAssignmentService.IsAssignedBefore(memberID, taskID);
            if (isAssigned)
                return RedirectToAction("Details", "ClubTasks", new { id = taskID, error = "Task is already assigned for this member" });
            TaskAssignment assign = new TaskAssignment()
            {
                TaskId = taskID,
                MembershipId = memberID
            };
            await _taskAssignmentService.AddTaskAssignmentAsync(assign);
            var member = await _memberService.GetClubMemberAsync(memberID);
            Notification newNoti = new Notification()
            {
                Message = $"You have been assigned with a new task in {member.Club.ClubName} event",
                Location = "Tasks",
                UserId = member.UserId
            };
            await _sender.Notify(newNoti, newNoti.UserId);
            return RedirectToAction("Details", "ClubTasks", new { id = taskID });
        }

        public async Task<IActionResult> Receive(int assignID, string status)
        {
            var assign = await _taskAssignmentService.GetTaskAssignmentAsync(assignID);
            assign.Status = status;
            await _taskAssignmentService.UpdateTaskAssignmentAsync(assign);
            return RedirectToAction("Index", "TaskAssignments");
        }

        public async Task<IActionResult> Delete(int assignID)
        {
            var assign = await _taskAssignmentService.GetTaskAssignmentAsync(assignID);
            assign.Status = "Unassigned";
            await _taskAssignmentService.UpdateTaskAssignmentAsync(assign);
            Notification newNoti = new Notification()
            {
                Message = $"You have been unassigned from a task",
                Location = "Tasks",
                UserId = assign.Membership.UserId
            };
            await _sender.Notify(newNoti, newNoti.UserId);
            return RedirectToAction("Details", "ClubTasks", new { id = assign.TaskId});
        }

        // GET: TaskAssignments
        public async Task<IActionResult> Index()
        {
            var userID = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tasks = await _taskAssignmentService.GetPersonalTaskAssignmentsAsync(userID);
            return View(tasks);
        }
    }
}
