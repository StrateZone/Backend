using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<TablesAppointmentRequest>> CheckAppointmentAvailability(AppointmentRequest request);
        Task<AppointmentModel> CreateAppointmentAsync(AppointmentRequest request);
        Task<AppointmentModel> DeleteAppointmentAsync(int id);
        Task<AppointmentModel> GetAppointmentByIdAsync(int id);
        Task<PagedList<AppointmentModel>> GetAppointmentsAsync(AppointmentParameters parameters);
        Task<PagedList<AppointmentModel>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int id);
        Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id);
    }
}