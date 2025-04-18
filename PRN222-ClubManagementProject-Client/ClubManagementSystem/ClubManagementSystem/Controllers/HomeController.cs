using System.Diagnostics;
using BussinessObjects.Models.Dtos;
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

        private readonly IImageHelperService _imageHelperService;

        public HomeController(ILogger<HomeController> logger, IImageHelperService imageHelperService)
        {
            _logger = logger;
           
            _imageHelperService = imageHelperService;
         
        }

        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask; 
            return View();
        }      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
