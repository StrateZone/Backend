using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<List<Payment>> GetPaymentsByTablesAppointmentIdAsync(int id);
        Task<List<Payment>> GetPaymentsByUserIdAsync(int id);
        Task<Payment> UpdatePaymentAsync(Payment payment, int id);
    }
}