using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IVoucherService
    {
        Task<VoucherModel> CreateVoucherAsync(VoucherRequest voucher);
        Task<VoucherModel> DeleteAsync(int id);
        Task<VoucherModel> GetByIdAsync(int id);
        Task<VoucherModel> GetVoucherByPaymentid(int paymentId);
        Task<PagedList<VoucherModel>> GetVouchersAsync(TablesAppointmentParameters parameters);
        Task<VoucherModel> UpdateVoucherAsync(VoucherModel voucher, int id);
    }
}
