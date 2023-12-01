using Microsoft.AspNetCore.Mvc;

namespace trivesta.Controllers
{
    public class RoomController : Controller
    {
        [Route("/room")]
        public IActionResult Index(string type)
        {
            if (string.IsNullOrEmpty(type))
            {

            }
            return View();
        }
        public IActionResult Chat(string roomID)
        {
            if (string.IsNullOrEmpty(roomID))
            {

            }
            return View();
        }
    }
}
