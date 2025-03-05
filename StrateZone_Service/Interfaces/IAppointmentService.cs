using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentModel> CreateAppointmentAsync(AppointmentRequest request);
        Task<AppointmentModel> DeleteAppointmentAsync(int id);
        Task<AppointmentModel> GetAppointmentByIdAsync(int id);
        Task<List<AppointmentModel>> GetAppointmentsAsync();
        Task<List<AppointmentModel>> GetAppointmentsByUserIdAsync(int userId);
        Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id);
    }
}