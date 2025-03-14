using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentModel> CreateAppointmentAsync(CustomModels.RequestModels.AppointmentRequest request);
        Task<AppointmentModel> DeleteAppointmentAsync(int id);
        Task<AppointmentModel> GetAppointmentByIdAsync(int id);
        Task<PagedList<AppointmentModel>> GetAppointmentsAsync(AppointmentParameters parameters);
        Task<PagedList<AppointmentModel>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int id);
        Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id);
    }
}