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
using Azure;
using System.Xml.Linq;
using StrateZone_Service.BusinessModels;
using Microsoft.AspNetCore.Http.Features;
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

        public async Task<Dictionary<string, object>> CreatePaymentRequestAsync(ZaloPayRequest zaloPayRequest)
        {
            var appId = _configuration["ZaloPay:AppId"];
            var key1 = _configuration["ZaloPay:Key1"];
            var endpoint = _configuration["ZaloPay:CreateOrderUrl"];
            var callbackUrl = _configuration["ZaloPay:CallbackUrl"];

            Random rnd = new Random();
            var app_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var embed_data = new
            {
                redirecturl = zaloPayRequest.ReturnUrl
            };
            var items = new List<ZaloPayItem>
            {
                new ZaloPayItem { ItemId = "DEPO", ItemName = "Deposite: " + zaloPayRequest.Amount, ItemPrice = zaloPayRequest.Amount, ItemQuantity = 1 }
            };
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.
            var param = new Dictionary<string, string>();

            param.Add("app_id", appId);
            param.Add("app_user", zaloPayRequest.UserId.ToString());
            param.Add("app_time", app_time);
            param.Add("amount", zaloPayRequest.Amount.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", zaloPayRequest.Description);
            param.Add("callback_url", callbackUrl);
            //param.Add("bank_code", "zalopayapp");

            var data = appId + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(endpoint, param);

            foreach (var entry in result)
            {
                Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            }

            return result;
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



                //// kiểm tra callback hợp lệ (đến từ ZaloPay server)
                //if (!reqMac.Equals(mac))
                //{
                //    // callback không hợp lệ
                //    result["return_code"] = -1;
                //    result["return_message"] = "mac not equal";
                //}
                //else
                //{

                //}




                // thanh toán thành công
                // merchant cập nhật trạng thái cho đơn hàng
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                //var rawItemJson = dataJson.GetProperty("item").GetString(); // "item" là chuỗi JSON

                // Giải mã "item" từ chuỗi JSON thành danh sách object
                var itemsStr = dataJson["item"];
                var items = (List<ZaloPayItem>)JsonConvert.DeserializeObject<List<ZaloPayItem>>(itemsStr);

                // Lấy danh sách item name
                var itemName = items.FirstOrDefault()?.ItemName;
                var userId = Convert.ToInt32(dataJson["app_user"]);
                var amount = Convert.ToDecimal(dataJson["amount"]);

                await _transactionRepository.SaveTransaction(new Transaction
                {
                    OfUser = userId,
                    Amount = amount,
                    ReferenceId = dataJson["zp_trans_id"].ToString(),
                    Content = "Transaction for: " + itemName,
                    TransactionType = TransactionType.deposit,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                });

                Wallet checkWallet = await _walletRepository.GetWalletByUserIdAsync(userId);
                if (checkWallet != null)
                {
                    checkWallet.Balance += amount;
                    await _walletRepository.UpdateWalletAsync(checkWallet, checkWallet.WalletId);
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
