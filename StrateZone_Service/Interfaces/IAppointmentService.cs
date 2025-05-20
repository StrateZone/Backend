using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<TablesAppointmentRequest>> CheckAppointmentAvailability(AppointmentRequest request);
        Task<AppointmentModel> CreateAppointmentAsync(AppointmentRequest request);
        Task<AppointmentModel> DeleteAppointmentAsync(int id);
        Task<AppointmentResponse> GetAppointmentByIdAsync(int id);
        Task<PagedList<AppointmentResponse>> GetAppointmentsAsync(AppointmentParameters parameters);
        Task<PagedList<AppointmentResponse>> GetAllAppointmentsAsync(AppointmentAdminParameters parameters);
        Task<PagedList<AppointmentResponse>> GetAllAppointmentsCheckinAsync(AppointmentAdminParameters parameters);
        Task<PagedList<AppointmentResponse>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int id);
        Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id);
        Task<AppointmentModel> UpdateAppointmentPriceAsync(int id);
        Task<TablesAppointmentModel> RefundAppointment100Async(int tableAppointmentId, int userId);
        Task<int> UpdateStatusForAppointmentBasedOnTablesAppointments();
    }
}