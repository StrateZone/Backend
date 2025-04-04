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
    public interface ISystemService
    {
        Task<TimeOnly> GetClosingHourAsync(int id);
        Task<TimeOnly> GetClosingHourOnDateAsync(int id, DateOnly date);
        Task<TimeOnly> GetOpeningHourAsync(int id);
        Task<TimeOnly> GetOpeningHourOnDateAsync(int id, DateOnly date);
        Task<List<SystemModel>> GetSystemsAsync();
        Task<SystemModel> GetSystemsByIdAsync(int id);
        Task<SystemModel> UpdateSystemWorkingTimeAsync(int id, TimeOnly openTime, TimeOnly closeTime);
        Task<AbnormalDayModel> AddAbnormalDayAsync(AbnormalDayRequest abnormalDay);
        Task<AbnormalDayModel> UpdateAbnormalDayAsync(AbnormalDayModel abnormalDay, int id);
        Task<AbnormalDayModel> DeleteAbnormalDayAsync(int id);
        Task<PagedList<AbnormalDayModel>> GetAbnormalDaysAsync(int id, TablesAppointmentParameters parameters);
    }
}
