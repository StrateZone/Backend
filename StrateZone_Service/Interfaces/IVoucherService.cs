using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IVoucherService
    {
        Task<VoucherModel> CreateSampleVoucherAsync(SampleVoucherRequest voucher);
        Task<VoucherModel> CreateVoucherFromSampleAsync(UserVoucherRequest voucher);
        Task<VoucherModel> DeleteAsync(int id);
        Task<VoucherModel> GetByIdAsync(int id);
        Task<VoucherModel> GetVoucherByPaymentid(int paymentId);
        Task<PagedList<VoucherModel>> GetVouchersAsync(TablesAppointmentParameters parameters);
        Task<PagedList<VoucherModel>> GetVouchersByUserIdAsync(TablesAppointmentParameters parameters, int userId);
        Task<PagedList<VoucherModel>> GetSampleVouchersAsync(TablesAppointmentParameters parameters);
        Task<VoucherModel> UpdateVoucherAsync(VoucherModel voucher, int id);
        Task<List<VoucherModel>> UpdateVouchersAsync(List<VoucherModel> vouchers);
        Task<List<VoucherModel>> UseVouchersAsync(List<int> voucherIds, int userId);
    }
}
