using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IPriceService
    {
        Task<PriceModel> GetMembershipPriceAsync();
        Task<PriceModel> GetPriceOfGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<PriceModel> GetPriceOfRoomTypeAsync(PostgreEnums.RoomType roomType);
        Task<PriceModel> GetPriceOfCourseAsync(int courseId);
        Task<decimal> GetPriceOfAppointmentAsync(int appointmentId);
        Task<PriceModel> GetProductPriceByIdAsync(int productId);
        Task<decimal> GetPriceOfAppointmentFromAppointmentRequestAsync(int[] tableIds, DateTime FromTime, DateTime ToTime);
        Task<List<decimal>> GetDetailedPriceOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime);
        Task<decimal> GetPriceOfTablesAppointmentAsync(TablesAppointmentModel tablesAppointment);
        Task<decimal> GetPriceOfAppointmentAsync(AppointmentModel appointment);
        Task<PagedList<PriceModel>> GetServicePricesAsync(PriceParameters parameters);
        Task<PriceModel> GetTeachingSalaryAsync();
        Task<PriceModel> UpdatePriceAsync(PriceModel priceModel, int id);
    }
}