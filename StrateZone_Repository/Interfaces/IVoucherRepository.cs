using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IVoucherRepository
    {
        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<Voucher> DeleteAsync(int id);
        Task<Voucher> GetByIdAsync(int id);
        Task<Voucher> GetVoucherByPaymentid(int paymentId);
        Task<PagedList<Voucher>> GetVouchersAsync(TablesAppointmentParameters parameters);
        Task<PagedList<Voucher>> GetSampleVouchersAsync(TablesAppointmentParameters parameters);
        Task<PagedList<Voucher>> GetVouchersByUserIdAsync(TablesAppointmentParameters parameters, int userid);
        Task<Voucher> UpdateVoucherAsync(Voucher voucher, int id);
    }
}