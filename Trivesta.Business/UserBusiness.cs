using App.Services;
using Trivesta.Model;

namespace Trivesta.Business
{
    public class UserBusiness
    {
        public UserBusiness()
        {
            
        }
        public async Task<ResponseMessage<string>> Recharge(string transaction_id)
        {
            ResponseMessage<string> responseMessage = new ResponseMessage<string>();

            //try
            //{
            //    var val = await FlutterwaveServices.VerifyTransactionAsync(transaction_id);

            //    if (val.StatusCode != 200)
            //    {
            //        responseMessage.StatusCode = 201;
            //        responseMessage.Message = val.Message;
            //        return responseMessage;
            //    }

            //    var res = val.Data.Data;
            //    User user = await _unitOfWork.Users.GetUserByUserNameOrEmail(res.Customer.Email);
            //    Wallet wallet = await _unitOfWork.Wallets.GetByUser(user.ID);
            //    if (wallet == null)
            //    {
            //        responseMessage.StatusCode = 209;
            //        responseMessage.Message = "Failed";
            //        return responseMessage;
            //    }
            //    var transaction = await _unitOfWork.Transactions.GetByUserIDAndContextID(transaction_id);

            //    if (transaction != null)
            //    {
            //        responseMessage.StatusCode = 209;
            //        responseMessage.Message = "Invalid Transaction";
            //        return responseMessage;
            //    }

            //    wallet.Coins += res.Amount;
            //    _unitOfWork.Wallets.Update(wallet);

            //    await _transactionBusiness.Create(res.Amount, $"Recharged Wallet ₦{res.Amount}", "Wallet", "border-success", user.ID, wallet.Coins, res.Tx_ref, transaction_id);
            //    //var val = FlutterwaveServices.Pay(user, cardNo, cvv, expiry, Guid.NewGuid().ToString(), (int)amount, pin);

            //    if (await _unitOfWork.Commit() > 0)
            //    {
            //        responseMessage.StatusCode = 200;
            //        responseMessage.Message = "Account Recharged Successfully!";
            //    }
            //    else
            //    {
            //        responseMessage.StatusCode = 201;
            //        responseMessage.Message = "Account Recharge failed";
            //    }
            //}
            //catch (Exception e)
            //{
            //    FileService.WriteToFile("\n\n" + e, "ErrorLogs");
            //    responseMessage.StatusCode = 209;
            //    responseMessage.Message = "Failed";
            //}
            return responseMessage;
        }


    }
}