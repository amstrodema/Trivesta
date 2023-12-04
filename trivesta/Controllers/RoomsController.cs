using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using trivesta.Services;
using Trivesta.Business;
using Trivesta.Model.ViewModel;

namespace trivesta.Controllers
{
    public class RoomsController : Controller
    {
        private readonly RoomBusiness _roomBusiness;
        private readonly LoginValidator _loginValidator;

        public RoomsController(RoomBusiness roomBusiness, LoginValidator loginValidator)
        {
            _roomBusiness = roomBusiness;
            _loginValidator = loginValidator;
        }
        [Route("/rooms")]
        public async Task<IActionResult> Index(string t)
        {
            var roomsVM = new RoomsVM();
            var roomType = await _roomBusiness.GetType(t);
            if (roomType == null)
            {
                roomsVM = await _roomBusiness.GetRoomsVM();
                return View(roomsVM);
            }
            roomsVM = await _roomBusiness.GetRoomsVM(roomType.Tag);
            roomsVM.RoomType = roomType;
            roomsVM.t = t.ToLower();
            roomsVM.LoggedInUser = await _loginValidator.GetUserAuth();

            return View("specific", roomsVM);
        }
        public async Task<IActionResult> Random()
        {
            var val = await _roomBusiness.RandomRoom();
            return RedirectToAction("index","room", new { t = val.Code });
        }
        public async Task<IActionResult> Create(string t)
        {
            if (! await _loginValidator.IsLoggedInAuth())
            {
                TempData["MessageError"] = "Login required!";
                return RedirectToAction("index", "home");
            }
            RoomsVM roomsVM = new RoomsVM();

            if (string.IsNullOrEmpty(t))
            {
                t = "public";
            }

            roomsVM.RoomType = await _roomBusiness.GetType(t);

            if (roomsVM.RoomType == null)
            {
                return RedirectToAction("Index");
            }
            TempData["RoomType.Tag"] = roomsVM.RoomType.Tag;
            roomsVM.t = t.ToLower();
            if (roomsVM.RoomType.IsMustLogIn)
            {
                return View("CreateForUsers",roomsVM);
            }
           
            return View(roomsVM);
        }
        [HttpPost]
        public async Task<IActionResult> Post(RoomsVM roomsVM, IFormFile image)
        {
            try
            {
                var user = await _loginValidator.GetUserAuth();
                if (user == null)
                {
                    TempData["MessageError"] = "Login required!";
                    return RedirectToAction("index", "home");
                }

                var res = await _roomBusiness.Create(roomsVM, user, image /*, TempData["RoomType.Tag"].ToString()*/);
                if (res.StatusCode != 200)
                {
                    TempData["MessageError"] = res.Message;
                    return RedirectToAction("Create", new { t = res.Data.Tag });
                }
                else
                {
                    TempData["MessageSuccess"] = res.Message;
                }


                return RedirectToAction("Index", new { t = res.Data.Tag });
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
