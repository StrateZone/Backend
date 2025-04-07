using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<AppointmentModel>> CreatePaymentBooking(AppointmentModel appointment);
        Task<ApiResponse<AppointmentrequestModel>> CreateAppointmentRequestPaymentBooking(AppointmentrequestPaymentRequest appointmentrequestModel);
        Task<ApiResponse<TablesAppointmentModel>> CreateTablesAppointmentPaymentBooking(TablesAppointmentPaymentRequest appointmentrequestModel);
        Task<PaymentModel> CreatePaymentAsync(PaymentModel payment);
        Task<List<PaymentModel>> GetPaymentsByTablesAppointmentIdAsync(int id);
        Task<PagedList<PaymentModel>> GetPaymentsAsync(StrateZone_Repository.Parameters.PaymentParameters parameters);
        Task<PagedList<PaymentModel>> GetPaymentsByUserIdAsync(int id, StrateZone_Repository.Parameters.PaymentParameters parameters);
        Task<PaymentModel> UpdatePaymentAsync(PaymentModel payment, int id);
    }
}
