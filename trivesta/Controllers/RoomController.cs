using Microsoft.AspNetCore.Mvc;

namespace trivesta.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
