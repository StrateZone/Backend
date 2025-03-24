using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IZaloPayService
    {
        Task<ZaloPayResponse> CreatePaymentRequestAsync(ZaloPayRequest zaloPayRequest);
        //Task<bool> HandleCallbackAsync(ZaloPayCallbackModel callbackData);
    }
}
