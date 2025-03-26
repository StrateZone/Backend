using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<AppointmentModel>> CreatePaymentBooking(AppointmentModel appointment);
        Task<PaymentModel> CreatePaymentAsync(PaymentModel payment);
        Task<List<PaymentModel>> GetPaymentsByTablesAppointmentIdAsync(int id);
        Task<List<PaymentModel>> GetPaymentsByUserIdAsync(int id);
        Task<PaymentModel> UpdatePaymentAsync(PaymentModel payment, int id);
    }
}
