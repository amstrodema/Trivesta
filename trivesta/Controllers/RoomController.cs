using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using trivesta.Services;
using Trivesta.Business;

namespace trivesta.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomBusiness _roomBusiness;
        private readonly LoginValidator _loginValidator;
        private readonly UserBusiness _userBusiness;
        public RoomController(RoomBusiness roomBusiness, LoginValidator loginValidator, UserBusiness userBusiness)
        {
            _roomBusiness = roomBusiness;
            _loginValidator = loginValidator;
            _userBusiness = userBusiness;
        }
        [Route("/room")]
        public async Task<IActionResult> Index(string t)
        {
            if (string.IsNullOrEmpty(t))
            {
                return RedirectToAction("Index", "Rooms");
            }

            var val = await _roomBusiness.GetRoomVM(t);
            if (val.Room == null) { return RedirectToAction("Index", "Rooms"); }

            return View(val);
        }
        public async Task<IActionResult> Chat(string roomID, string t)
        {
            var user = _loginValidator.GetUser();
            if (string.IsNullOrEmpty(roomID) && !string.IsNullOrEmpty(t))
            {
                return RedirectToAction("Index", "Room", new {t = t });
            }
            else if (string.IsNullOrEmpty(roomID))
            {
                return RedirectToAction("Index", "Rooms");
            }

            if (user == null)
            {
                var rez = await _userBusiness.Guest();
                if (rez.StatusCode != 200)
                {
                    TempData["MessageError"] = rez.Message;
                    return RedirectToAction("Index", new { t = roomID });
                }
                else
                {
                    _loginValidator.SetSession("user", JsonConvert.SerializeObject(rez.Data));
                    user = rez.Data;
                }
                //return View("Guest");
            }

            var res = await _roomBusiness.ChatRoomsVM(roomID, user);

            if (res.StatusCode != 200)
            {
                TempData["MessageError"] = res.Message;
                return RedirectToAction("Index", new {  t = roomID });
            }
            //else
            //{
            //    TempData["MessageSuccess"] = res.Message;
            //}

            return View(res.Data);
        }
    }
}
