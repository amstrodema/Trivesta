using Microsoft.AspNetCore.Mvc;

namespace trivesta.Controllers
{
    public class ChatController : Controller
    {
        [Route("/chat")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
