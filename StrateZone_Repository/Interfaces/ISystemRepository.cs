using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface ISystemRepository
    {
        Task<TimeOnly> GetClosingHourAsync(int id);
        Task<TimeOnly> GetClosingHourOnDateAsync(int id, DateOnly date);
        Task<TimeOnly> GetOpeningHourAsync(int id);
        Task<TimeOnly> GetOpeningHourOnDateAsync(int id, DateOnly date);
        Task<List<Entities.System>> GetSystemsAsync();
        Task<Entities.System> GetSystemsByIdAsync(int id);
        Task<Entities.System> UpdateSystemWorkingHoursAsync(int id, TimeOnly openTime, TimeOnly closeTime);
        Task<Entities.System> UpdateAppointmentTimeRulesAsync(int id, decimal refund100Time, decimal incomingTime, int minutesCheckin);
        Task<AbnormalDay> AddAbnormalDayAsync(AbnormalDay abnormalDay);
        Task<AbnormalDay> UpdateAbnormalDayAsync(AbnormalDay abnormalDay, int id);
        Task<AbnormalDay> DeleteAbnormalDayAsync(int id);
        Task<PagedList<AbnormalDay>> GetAbnormalDaysAsync(int id, TablesAppointmentParameters parameters);
        Task<decimal> GetAppointmentRefund100TimeInHoursAsync(int id);
        Task<decimal> GetAppointmentIncomingTimeInHoursAsync(int id);
        Task<int> GetAppointmentCheckinTimeInMinuesAsync(int id);
    }
}