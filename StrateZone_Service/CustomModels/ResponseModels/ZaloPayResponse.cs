using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class ZaloPayResponse
    {
        [JsonProperty("return_code")]
        public int ReturnCode { get; set; }

        [JsonProperty("return_message")]
        public string ReturnMessage { get; set; }

        [JsonProperty("zp_trans_token")]
        public string ZpTransToken { get; set; }

        [JsonProperty("order_url")]
        public string OrderUrl { get; set; }

        [JsonProperty("order_token")]
        public string OrderToken { get; set; }

        [JsonProperty("app_trans_id")]
        public string AppTransId { get; set; }
    }
}
