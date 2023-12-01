using Microsoft.AspNetCore.Mvc;
using Trivesta.Model.ViewModel;

namespace trivesta.Controllers
{
    public class RoomsController : Controller
    {
        public RoomsController()
        {
            
        }
        [Route("/rooms")]
        public IActionResult Index(string t)
        {
            RoomsVM roomsVM = new RoomsVM();
            if (string.IsNullOrEmpty(t) || (t.ToLower() != "public" && t.ToLower() != "private" && t.ToLower() != "classy" && t.ToLower() != "personal"))
            {
                return View(roomsVM);
            }
            roomsVM.t = t.ToLower();
            return View("specific", roomsVM);
        }
        public IActionResult Random()
        {
            return RedirectToAction("Index");
        }
        public IActionResult Create(string t)
        {
            RoomsVM roomsVM = new RoomsVM();
            if (string.IsNullOrEmpty(t) || (t.ToLower() != "public" && t.ToLower() != "private" && t.ToLower() != "classy" && t.ToLower() != "personal"))
            {
                return RedirectToAction("Index");
            }
            roomsVM.t = t.ToLower();
            if (t.ToLower() == "private" ||  t.ToLower() == "classy")
            {
                return View("CreateForUsers",roomsVM);
            }
           
            return View(roomsVM);
        }
    }
}
