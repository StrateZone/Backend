using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface IPriceRepository
    {
        Task<Price> GetMembershipPriceAsync();
        Task<Price> GetPriceOfGameTypeAsync(GameTypeEnum gameType);
        Task<Price> GetPriceOfRoomTypeAsync(RoomType roomType);
        Task<decimal> GetPriceOfAppointmentAsync(int appointmentId);
        Task<Price> GetPriceOfCourseAsync(int courseId);
        Task<Price> GetProductPriceByIdAsync(int productId);
        Task<decimal> GetPriceOfAppointmentTablesFromTimeRangeAsync(int[] tableId, DateTime FromTime, DateTime ToTime);
        Task<List<decimal>> GetDetailedPriceOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime);
        Task<decimal> GetPriceOfTablesAppointmentAsync(TablesAppointment tablesAppointment);
        Task<decimal> GetPriceOfAppointmentAsync(Appointment appointment);
        Task<PagedList<Price>> GetServicePrices(PriceParameters parameters);
        Task<Price> GetTeachingSalaryAsync();
        Task<Price> UpdatePriceAsync(Price price, int id);
    }
}