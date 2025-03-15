using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class RefreshTokenResponse
    {
        public string NewToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
