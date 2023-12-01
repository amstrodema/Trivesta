using Microsoft.AspNetCore.Mvc;

namespace trivesta.Controllers
{
    public class ManagerController : Controller
    {
        public ManagerController()
        {
            
        }
        public IActionResult Credit()
        {
            return View();
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
