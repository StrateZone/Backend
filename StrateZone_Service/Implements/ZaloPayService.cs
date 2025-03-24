using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ZaloPay.Helper.Crypto;
using ZaloPay.Helper;
using Azure;

namespace StrateZone_Service.Implements
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public ZaloPayService(HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
        }

        public async Task<ZaloPayResponse> CreatePaymentRequestAsync(ZaloPayRequest zaloPayRequest)
        {
            var appId = _configuration["ZaloPay:AppId"];
            var key1 = _configuration["ZaloPay:Key1"];
            var endpoint = _configuration["ZaloPay:CreateOrderUrl"];
            var random = new Random();

            var transId = DateTime.Now.ToString("yyMMdd") + "_" + random.Next(1000000);

            var appUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest_user";

            var embedData = JsonConvert.SerializeObject(new { redirectUrl = zaloPayRequest.ReturnUrl }, Formatting.None);

            var items = "[]";

            var requestData = new Dictionary<string, string>();

            requestData.Add("app_id", appId);
            requestData.Add("app_user", appUser);
            requestData.Add("app_time", Utils.GetTimeStamp().ToString());
            requestData.Add("amount", zaloPayRequest.Amount.ToString());
            requestData.Add("app_trans_id", transId);
            requestData.Add("embed_data", embedData);
            requestData.Add("item", items);
            requestData.Add("description", zaloPayRequest.Description);
            //requestData.Add("bank_code", "zalopayapp");

            Console.WriteLine(JsonConvert.SerializeObject(requestData, Formatting.Indented));

            var data = appId + "|" + requestData["app_trans_id"] + "|" + requestData["app_user"] + "|" + requestData["amount"] + "|"
                + requestData["app_time"] + "|" + requestData["embed_data"] + "|" + requestData["item"];

            Console.WriteLine("Data string for MAC: " + data);
            

            requestData.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            Console.WriteLine("Generated MAC: " + requestData["mac"]);

            Console.WriteLine("Request Data: " + JsonConvert.SerializeObject(requestData, Formatting.Indented));

            var response = await HttpHelper.PostFormAsync(endpoint, requestData);

            Console.WriteLine("ZaloPay Response: " + JsonConvert.SerializeObject(response, Formatting.Indented));


            //foreach (var entry in response)
            //{
            //    Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            //}

            var jsonResponse = JsonConvert.SerializeObject(response);
            var zaloPayResponse = JsonConvert.DeserializeObject<ZaloPayResponse>(jsonResponse);

            return zaloPayResponse;
        }


        //public async Task<bool> HandleCallbackAsync(ZaloPayCallbackModel callbackData)
        //{
        //    if (callbackData.ReturnCode != 1)
        //    {
        //        return false;
        //    }

        //    var user = await _userRepository.GetUserByUsernameAsync(callbackData.AppUser);
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    var transaction = new Transaction
        //    {
        //        OfUser = user.UserId,
        //        ReferenceId = callbackData.AppTransId,
        //        Content = "Balance changed",
        //        Amount = callbackData.Amount,
        //        TransactionType = TransactionType.deposit,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    await _transactionRepository.SaveTransaction(transaction);

        //    var userWallet = await _walletRepository.GetByUserIdAsync(user.UserId);
        //    if (userWallet == null) return false;

        //    userWallet.Balance += callbackData.Amount;

        //    await _walletRepository.UpdateAsync(userWallet);

        //    return true;
        //}
        //private string ComputeHmacSHA256(string data, string key)
        //{
        //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        //    {
        //        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        //        return BitConverter.ToString(hash).Replace("-", "").ToLower();
        //    }
        //}
    }
}
