using BussinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interface;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ClubManagementSystem.Controllers.Filter
{
    public class ClubAuthorization : IAsyncActionFilter, IAsyncResultFilter
    {
        private readonly IClubMemberService _memberService;

        public ClubAuthorization(IClubMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ActionArguments.TryGetValue("clubID", out var clubIDObj) || clubIDObj == null)
            {
                context.Result = new RedirectToActionResult("Index", "Clubs", null);
                return;
            }
            context.HttpContext.Items["ClubID"] = clubIDObj;
            // Proceed to the action if the user is a member
            await next();
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            int? clubID = controller.ViewBag.ClubID ?? controller.ViewData["ClubID"]?? context.HttpContext.Items["ClubID"] as int?;
            var userID = Int32.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if(clubID == null)
            {
                context.Result = new RedirectToActionResult("Index", "Clubs", null);
                return;
            }

            // Check if the user is a member asynchronously
            var isMember = await _memberService.IsClubMember(clubID.Value, userID);
            if (!isMember)
            {
                // Redirect to Clubs/Details if the user is not a member of the club
                context.Result = new RedirectToActionResult("Details", "Clubs", new { id = clubID });
                return;
            }

            // Proceed to the next stage in the filter pipeline
            await next();
        }
    }
}
