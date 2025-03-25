using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System.Security.Claims;
using ZaloPay.Helper;
using ZaloPay.Helper.Crypto;
using static StrateZone_Repository.Parameters.PostgreEnums;

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

            var transId = DateTime.UtcNow.AddHours(7).ToString("yyMMdd") + "_" + random.Next(1000000);

            var appUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest_user";

            var embedData = JsonConvert.SerializeObject(new { redirectUrl = zaloPayRequest.ReturnUrl }, Formatting.None);

            var items = new List<ZaloPayItem>
            {
                new ZaloPayItem { ItemId = "Depo", ItemName = "Deposite " + zaloPayRequest.Amount, ItemPrice = zaloPayRequest.Amount, ItemQuantity = 1 }
            };

            // Chuyển đổi sang JSON
            string itemJson = JsonConvert.SerializeObject(items);

            var requestData = new Dictionary<string, string>();

            requestData.Add("app_id", appId);
            requestData.Add("app_user", appUser);
            requestData.Add("app_time", ZaloPay.Helper.Utils.GetTimeStamp().ToString());
            requestData.Add("amount", zaloPayRequest.Amount.ToString());
            requestData.Add("app_trans_id", transId);
            requestData.Add("embed_data", embedData);
            requestData.Add("item", itemJson);
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


        public async Task<Dictionary<string, object>> HandleCallbackAsync(dynamic callbackData)
        {
            var result = new Dictionary<string, object>();
            var key2 = _configuration["ZaloPay:Key2"];

            Console.WriteLine($"Callback Received: {JsonConvert.SerializeObject(callbackData)}");

            try
            {
                var dataStr = Convert.ToString(callbackData["data"]);
                var reqMac = Convert.ToString(callbackData["mac"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    // thanh toán thành công
                    // merchant cập nhật trạng thái cho đơn hàng
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                    var rawItemJson = dataJson.GetProperty("item").GetString(); // "item" là chuỗi JSON

                    // Giải mã "item" từ chuỗi JSON thành danh sách object
                    var items = JsonConvert.DeserializeObject<List<ZaloPayItem>>(rawItemJson);

                    // Lấy danh sách item name
                    var itemName = items.FirstOrDefault()?.ItemName;
                    var userId = dataJson["app_user"];
                    var amount = dataJson["amount"];

                    await _transactionRepository.SaveTransaction(new Transaction
                    {
                        OfUser = userId,
                        Amount = amount,
                        ReferenceId = dataJson["zp_trans_id"],
                        Content = "Transaction for: " + itemName,
                        CreatedAt = DateTime.UtcNow,
                    });

                    Wallet checkWallet = await _walletRepository.GetWalletByUserIdAsync(userId);
                    if (checkWallet != null)
                    {
                        checkWallet.Balance += amount;
                        await _walletRepository.UpdateWalletAsync(checkWallet, userId);
                    }
                    else
                    {
                        var newWallet = new Wallet
                        {
                            UserId = userId,
                            Balance = amount,
                            Status = WalletStatus.active
                        };

                        await _walletRepository.CreateWalletAsync(newWallet);
                    }

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }

            // thông báo kết quả cho ZaloPay server
            return result;
        }




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
