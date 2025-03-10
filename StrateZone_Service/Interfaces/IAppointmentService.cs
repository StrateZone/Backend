using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentModel> CreateAppointmentAsync(CustomModels.RequestModels.AppointmentRequest request);
        Task<AppointmentModel> DeleteAppointmentAsync(int id);
        Task<AppointmentModel> GetAppointmentByIdAsync(int id);
        Task<List<AppointmentModel>> GetAppointmentsAsync();
        Task<List<AppointmentModel>> GetAppointmentsByUserIdAsync(int userId);
        Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id);
    }
}