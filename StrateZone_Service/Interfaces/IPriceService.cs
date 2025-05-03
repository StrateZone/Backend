using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Interfaces
{
    public interface IPriceService
    {
        Task<PriceModel> CreatePriceAsync(PriceModel priceModel);
        Task<PriceModel> GetMembershipPriceAsync();
        Task<PriceModel> GetPriceOfGameTypeAsync(string gameType);
        Task<PriceModel> GetPriceOfRoomTypeAsync(string roomType);
        Task<PriceModel> GetPriceOfCourseAsync(int courseId);
        Task<decimal> GetPriceOfAppointmentAsync(int appointmentId);
        Task<PriceModel> GetProductPriceByIdAsync(int productId);
        Task<Dictionary<int, decimal>> GetPricesPerHourEachGameTypeAsync();
        Task<Dictionary<string, decimal>> GetPricesPerHourEachRoomTypeAsync();
        Task<decimal> GetPriceOfAppointmentFromAppointmentRequestAsync(int[] tableIds, DateTime FromTime, DateTime ToTime);
        Task<List<decimal>> GetDetailedPriceOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime);
        Task<decimal> GetPriceOfTablesAppointmentAsync(TablesAppointmentModel tablesAppointment);
        Task<decimal> GetPriceOfAppointmentAsync(AppointmentModel appointment);
        Task<PagedList<PriceModel>> GetServicePricesAsync(PriceParameters parameters);
        Task<PriceModel> GetTeachingSalaryAsync();
        Task<PriceModel> UpdatePriceAsync(PriceModel priceModel, int id);
    }
}