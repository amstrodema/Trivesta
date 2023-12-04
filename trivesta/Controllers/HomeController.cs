using App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using trivesta.Models;
using Trivesta.Business;
using Trivesta.Model.ViewModel;

namespace trivesta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FlutterwaveServices _flutterwaveServices;
        private readonly GeneralBusiness _generalBusiness;

        public HomeController(ILogger<HomeController> logger, FlutterwaveServices flutterwaveServices, GeneralBusiness generalBusiness)
        {
            _logger = logger;
            _flutterwaveServices = flutterwaveServices;
            _generalBusiness = generalBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var val = await _generalBusiness.GetHomeVM();

            return View("Index", val);
        }
        [Route("/profile")]
        public IActionResult Profile()
        {
            return View();
        }
        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}