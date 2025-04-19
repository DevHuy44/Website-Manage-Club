using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClubManagementSystem.Controllers.Filter
{
    public class ClubAdminAuthorizeAttribute : ActionFilterAttribute, IAsyncAuthorizationFilter
    {
        private readonly List<string> _roles;
        public ClubAdminAuthorizeAttribute(string roles)
        {
            _roles = roles.Split(',').ToList();
        }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            int clubId;
            // First try to get clubId from RouteData
            if (!context.RouteData.Values.TryGetValue("id", out var clubIdValue) ||
                !int.TryParse(clubIdValue?.ToString(), out clubId))
            {
                // If not found in RouteData, check Query parameters
                var queryClubId = context.HttpContext.Request.Query["id"].ToString();
                if (!int.TryParse(queryClubId, out clubId))
                {
                    context.Result = new BadRequestObjectResult("Missing or invalid clubId parameter.");
                    return Task.CompletedTask;
                }
            }

            string claimType = $"ClubRole_{clubId}";
            var clubRoleClaim = user.Claims.FirstOrDefault(c => c.Type == claimType);

            bool hasRequiredRole = clubRoleClaim != null && _roles.Contains(clubRoleClaim.Value);


            if (!hasRequiredRole && !user.IsInRole("SystemAdmin"))
            {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
