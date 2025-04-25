using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace ClubManagementSystem.Controllers
{
    public class ClubStatsController : Controller
    {
        private readonly IClubStatsService _statsService;

        public ClubStatsController(IClubStatsService statsService)
        {
            _statsService = statsService;
        }

        // Action trả về View
        public IActionResult Index()
        {
            // Luôn lấy tháng hiện tại làm mặc định
            DateTime selectedMonth = DateTime.UtcNow;
            ViewBag.TargetMonth = selectedMonth.ToString("yyyy-MM");
            return View();
        }

        // Action trả về dữ liệu JSON cho biểu đồ
        [HttpGet]
        public async Task<IActionResult> GetStats(string targetMonth)
        {
            if (!DateTime.TryParse(targetMonth + "-01", out var month))
            {
                month = DateTime.UtcNow; // Mặc định là tháng hiện tại nếu không hợp lệ
            }

            var memberStats = await _statsService.GetMemberStatsAsync(month);
            var eventStats = await _statsService.GetEventStatsAsync(month);
            var postStats = await _statsService.GetPostStatsAsync(month);

            return Json(new
            {
                Members = memberStats,
                Events = eventStats,
                Posts = postStats
            });
        }
    }
}
