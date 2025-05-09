using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<List<Payment>> GetPaymentsByTablesAppointmentIdAsync(int id);
        Task MassUpdatePaymentsAsync(List<Payment> payments);
        Task<PagedList<Payment>> GetPaymentsAsync(PaymentParameters parameters);
        Task<PagedList<Payment>> GetPaymentsByUserIdAsync(int id, PaymentParameters parameters);
        Task<Payment> UpdatePaymentAsync(Payment payment, int id);
        Task<int> GetMembershipPaymentsWithinAMonthInYearAsync(int month, int year);
        Task<int> GetMembershipPaymentsWithinADayInYearAsync(int day, int month, int year);
    }
}