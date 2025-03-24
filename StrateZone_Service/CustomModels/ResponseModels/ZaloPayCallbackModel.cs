using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class ZaloPayCallbackModel
    {
        [JsonProperty("appid")]
        public int AppId { get; set; }

        [JsonProperty("app_user")]
        public string AppUser { get; set; }

        [JsonProperty("app_trans_id")]
        public string AppTransId { get; set; }

        [JsonProperty("app_time")]
        public long AppTime { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("embed_data")]
        public string EmbedData { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("zp_trans_id")]
        public long ZpTransId { get; set; }

        [JsonProperty("server_time")]
        public long ServerTime { get; set; }

        [JsonProperty("channel")]
        public int Channel { get; set; }

        [JsonProperty("merchant_user_id")]
        public string MerchantUserId { get; set; }

        [JsonProperty("user_fee_amount")]
        public decimal UserFeeAmount { get; set; }

        [JsonProperty("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("return_code")]
        public int ReturnCode { get; set; }

        [JsonProperty("return_message")]
        public string ReturnMessage { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }
    }
}
