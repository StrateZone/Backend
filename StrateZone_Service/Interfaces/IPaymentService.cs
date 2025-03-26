using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentModel> CreatePaymentAsync(PaymentModel payment);
        Task<List<PaymentModel>> GetPaymentsByTablesAppointmentIdAsync(int id);
        Task<List<PaymentModel>> GetPaymentsByUserIdAsync(int id);
        Task<PaymentModel> UpdatePaymentAsync(PaymentModel payment, int id);
    }
}
