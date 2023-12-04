using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using trivesta.Services;
using Trivesta.Business;
using Trivesta.Model;

namespace trivesta.Controllers
{
    public class ManagerController : Controller
    {
        private readonly LoginValidator _loginValidator;
        private readonly UserBusiness _userBusiness;

        public ManagerController(LoginValidator loginValidator, UserBusiness userBusiness)
        {
            _loginValidator = loginValidator;
            _userBusiness = userBusiness;
        }

        [Route("Manager")]
        public async Task<IActionResult> Index()
        {
            var user = await _loginValidator.GetUserAuth();

            if (user == null)
            {
                TempData["MessageError"] = "Login required!";
                return RedirectToAction("index", "home");
            }
            return View();
        }

        //public async Task<IActionResult> Guest(User user)
        //{
        //    var res = await _userBusiness.Guest();
        //    if (res.StatusCode != 200)
        //    {
        //        TempData["MessageError"] = res.Message;
        //    }
        //    else
        //    {
        //        _loginValidator.SetSession("user", JsonConvert.SerializeObject(res.Data));
        //        TempData["MessageSuccess"] = res.Message;
        //    }
        //    return View();
        //} 
        
        [HttpPost]
        public async Task<IActionResult> Login(string emailOrUserName, string password)
        {
            var res = await _userBusiness.Login(emailOrUserName, password);
            if (res.StatusCode != 200)
            {
                TempData["MessageError"] = res.Message;
                return RedirectToAction("index","home");
            }
            else
            {
                _loginValidator.SetSession("user", JsonConvert.SerializeObject(res.Data));
                TempData["MessageSuccess"] = res.Message;
            }
            return RedirectToAction("index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var res = await _userBusiness.Create(email, username, password);
            if (res.StatusCode != 200)
            {
                TempData["MessageError"] = res.Message;
            }
            else
            {
                TempData["MessageSuccess"] = res.Message;
            }

            return RedirectToAction("index", "home");
        }
        public IActionResult Credit()
        {
            return View();
        }
        public async Task<IActionResult> Setup()
        {
            var res = await _userBusiness.Setup();
            if (res.StatusCode != 200)
            {
                TempData["MessageError"] = res.Message;
            }
            else
            {
                TempData["MessageSuccess"] = res.Message;
            }

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> CreditWallet(string tx_ref, string transaction_id, string status)
        {
          //  var res = await _walletBusiness.Recharge(transaction_id);

            //if (res.StatusCode != 200)
            //{
            //    TempData["MessageError"] = res.Message;
            //}
            //else
            //{
            //    TempData["MessageSuccess"] = res.Message;
            //}

            return RedirectToAction("Index", "Dashboard");
        }

    }
}
