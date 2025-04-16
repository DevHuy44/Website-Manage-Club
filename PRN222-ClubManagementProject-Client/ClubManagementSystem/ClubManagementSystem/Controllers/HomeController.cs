using System.Diagnostics;
using BussinessObjects.Models.Dtos;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Services.Implementation;
using Services.Interface;

namespace ClubManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IClubService _clubService;
        private readonly IEventService _eventService;
        private readonly IImageHelperService _imageHelperService;

        public HomeController(ILogger<HomeController> logger, SignalRSender sender, IPostService postService, IClubService clubService, IImageHelperService imageHelperService, IEventService eventService)
        {
            _logger = logger;
            _postService = postService;
            _clubService = clubService;
            _imageHelperService = imageHelperService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = (await _postService.GetPostsAsync("Approved"))
              .OrderByDescending(p => p.CreatedAt)
              .Select(p => new PostDetailsDto
              {
                  PostId = p.PostId,
                  Title = p.Title,
                  Content = p.Content,
                  ImageBase64 = _imageHelperService.ConvertToBase64(p.Image, "png"),
                  CreatedAt = p.CreatedAt,
                  Status = p.Status,
                  LikeCount = p.Reactions.Count(r => r.IsLiked),
                  Club = new ClubDto
                  {
                      ClubId = p.ClubMember.Club.ClubId,
                      ClubName = p.ClubMember.Club.ClubName
                  }
              })
              .ToList();


            var clubs = (await _clubService.GetAllClubsApprovedAsync())
                 .OrderByDescending(c => c.CreatedAt)
                 .Select(c => new ClubDetailsViewDto
                 {
                     ClubId = c.ClubId,
                     ClubName = c.ClubName,
                     Description = c.Description,
                     CreatedAt = c.CreatedAt,
                     LogoBase64 = _imageHelperService.ConvertToBase64(c.Logo, "png")
                 })
                 .ToList();

            var events = await _eventService.GetOnGoingEvents();

            var viewModel = new HomeDto
            {
                Posts = posts,
                Clubs = clubs,
                Events = events
            };

            return View(viewModel);
        }      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
