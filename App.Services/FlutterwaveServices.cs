using System.Text;
using Rave.Models.MobileMoney;
using Rave.Models.VirtualCard;
using Rave.Models.Subaccount;
using Rave.Models.Tokens;
using Rave.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rave.api;
using Rave.Models.Charge;
using Rave.Models.Account;
using Rave.Models.Card;
using Rave.Models.Validation;
using NUnit.Framework;
using Rave;
using System.Net.NetworkInformation;
using System.Net.Http;
using Rave.Models.Banks;
using Trivesta.Model;

namespace App.Services
{
    public class FlutterwaveServices
    {

        private static bool IsLive = false;
        public static string PbKey = "FLWPUBK_TEST-9260215d762d804babaf5c85895bb344-X";
        private static string ScKey = "FLWSECK_TEST-0f27654687e572bba961856956264743-X";
        private static string Encryption = "FLWSECK_TEST95969e938682";
        private static RaveConfig raveConfig = new RaveConfig(PbKey, ScKey, false);
        private static CardParams payload;
        private static ChargeCard cardCharge;
        private static RaveResponse<Rave.Models.Card.ResponseData> cha;
        private readonly HttpClient _httpClient;

        public FlutterwaveServices(IHttpClientFactory httpClientFactory)
        {

            if (IsLive)
            {
                PbKey = "FLWPUBK-c715146f541da80f2202baa859c83bd0-X";
                ScKey = "FLWSECK-894363394ecdc2f5f9146d344c2249cd-18bb3e50026vt-X";
                Encryption = "894363394ecdee6e3ae94c84";
            }

        }
        public static ResponseMessage<string> Pay(User user, string cardNo, string cvv, string expiry, string tranxRef, int amt, string pin)
        {
            try
            {
                ResponseMessage<string> responseMessage = new ResponseMessage<string>();
                cardCharge = new ChargeCard(raveConfig);
                var exp = expiry.Split("-");
                payload = new CardParams(PbKey, ScKey, user.Username, user.Username, user.Email, amt, "NGN")
                {
                    CardNo = cardNo,
                    Cvv = cvv,
                    Expirymonth = exp[1],
                    Expiryyear = exp[0],
                    TxRef = tranxRef,
                    Pin = pin
                };

                cha = cardCharge.Charge(payload).Result;

                if (cha.Status.ToLower() != "success")
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = cha.Message;
                }
                else
                {
                    responseMessage.StatusCode = 200;
                }

                return responseMessage;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static async Task<ResponseMessage<Response>> VerifyTransactionAsync(string transactionId)
        {
            // Replace 'YOUR_FLW_PUBLIC_KEY' and 'YOUR_FLW_SECRET_KEY' with your actual Flutterwave public and secret keys
            //string publicKey = "YOUR_FLW_PUBLIC_KEY";
            //string secretKey = "YOUR_FLW_SECRET_KEY";
            ResponseMessage<Response> responseMessage = new ResponseMessage<Response>();

            // Replace 'https://api.flutterwave.com/v3/transactions/{transactionId}/verify' with the actual endpoint for transaction verification
            string apiUrl = $"https://api.flutterwave.com/v3/transactions/{transactionId}/verify";

            using (HttpClient client = new HttpClient())
            {
                // Set the Authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ScKey}");

                try
                {
                    // Perform the verification request
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response
                        // Note: You may need to use a JSON library like Newtonsoft.Json to deserialize the response
                        // For simplicity, I'm assuming a hypothetical Response class here
                        var verificationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseBody);

                        // Check the verification response
                        if (verificationResponse.Data.Status == "successful")
                        {
                            // Success! Confirm the customer's payment
                            responseMessage.StatusCode = 200;
                            responseMessage.Data = verificationResponse;
                            //  Console.WriteLine("Transaction verified successfully. Payment confirmed.");
                        }
                        else
                        {
                            // Inform the customer their payment was unsuccessful
                            responseMessage.StatusCode = 201;
                            responseMessage.Message = "Payment unsuccessful";
                            //      Console.WriteLine("Transaction verification failed. Payment unsuccessful.");
                        }
                    }
                    else
                    {
                        responseMessage.StatusCode = 201;
                        responseMessage.Message = "Payment unsuccessful";
                        // Console.WriteLine($"Transaction verification failed. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Payment unsuccessful";
                    // Console.WriteLine($"An error occurred: {e.Message}");
                }
            }
            return responseMessage;
        }


        public static ResponseMessage<Tuple<string, decimal>> OTP(string otp)
        {
            try
            {
                ResponseMessage<Tuple<string, decimal>> responseMessage = new ResponseMessage<Tuple<string, decimal>>();
                if (cha.Message == "AUTH_SUGGESTION" && cha.Data.SuggestedAuth == "PIN")
                {
                    //payload.Pin = pin;
                    payload.Otp = otp;
                    payload.SuggestedAuth = "PIN";
                    cha = cardCharge.Charge(payload).Result;
                }

                if (cha.Status.ToLower() != "success")
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = cha.Message;
                }
                else
                {
                    responseMessage.StatusCode = 200;
                    responseMessage.Data = Tuple.Create(payload.TxRef, payload.Amount);
                }

                return responseMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Transfer()
        {
            var banks = TransferService.GetBankList();
        }

        public async Task<ResponseMessage<string>> CreateTransfer(int accountNo)
        {
            // Replace with your actual Flutterwave API endpoint
            string apiUrl = "https://api.flutterwave.com/v3/transfers";
            var banks = TransferService.GetBankList();
            ResponseMessage<string> responseMessage = new ResponseMessage<string>();

            // Replace with your actual transfer data
            string jsonData = @"{
            ""account_bank"": ""044"",
            ""account_number"": ""0690000040"",
            ""amount"": 200,
            ""narration"": ""Payment for things"",
            ""currency"": ""NGN"",
            ""reference"": ""jh678b3kol1Z"",
            ""callback_url"": ""https://webhook.site/b3e505b0-fe02-430e-a538-22bbbce8ce0d"",
            ""debit_currency"": ""NGN""
        }";

            using (var content = new StringContent(jsonData, Encoding.UTF8, "application/json"))
            {
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    responseMessage.StatusCode = 200;
                    responseMessage.Message = "Completed!";
                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Request failed";
                    //return StatusCode((int)response.StatusCode, $"Request failed with status code {response.StatusCode}");
                }
            }

            return responseMessage;
        }
    }
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Tx_ref { get; set; }
        public string Flw_ref { get; set; }
        public string Device_fingerprint { get; set; }
        public string Currency { get; set; }
        public decimal Charged_amount { get; set; }
        public decimal App_fee { get; set; }
        public string Status { get; set; }
        public string Payment_type { get; set; }
        public string Narration { get; set; }
        public Customer Customer { get; set; }
        public Card Card { get; set; }
    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}