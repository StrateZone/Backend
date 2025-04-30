using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IAppointmentrequestRepository
    {
        Task<Appointmentrequest> CreateAppointmentRequestAsync(Appointmentrequest appointmentRequest);
        Task<List<Appointmentrequest>> CreateAppointmentRequestsAsync(List<Appointmentrequest> appointmentRequest);
        Task<Appointmentrequest> DeleteAppointmentRequestAsync(int id);
        Task<Appointmentrequest> GetAppointmentRequestByIdAsync(int id);
        Task<PagedList<Appointmentrequest>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId);
        Task<PagedList<Appointmentrequest>> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId);
        Task<List<Appointmentrequest>> GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(int userId, int tablesAppointmentId);
        Task<List<Appointmentrequest>> GetAppointmentRequestsByTablesAppointmentIdAsync(int tablesAppointmentId);
        Task<List<Appointmentrequest>> GetAppointmentRequestsByAppointmentIdAsync(int appointmentId);
        Task<List<Appointmentrequest>> GetCurrentAppointmentRequestsFromUserByUserAndTableAsync(int userId, int tableId, DateTime startTime, DateTime endTime);
        Task<Appointmentrequest> UpdateAppointmentRequestAsync(Appointmentrequest appointmentRequest, int id);
        Task<List<Appointmentrequest>> MassUpdateAppointmentRequestsAsync(List<Appointmentrequest> appointmentRequests);
        Task<int> UpdateExpiredAppointmentRequests();
        Task<Appointmentrequest> AcceptAppointmentrequestAsync(int id);
        Task<Appointmentrequest> RejectAppointmentrequestAsync(int id);
        Task<List<Appointmentrequest>> CancelAllSentRequestFromUserAsync(int userId);
        Task<List<Appointmentrequest>> CancelAllAppointmentRequestsFromUserOnTableAsync(int userId, int tableId, DateTime startTime, DateTime endTime);
        Task CancelAllSentRequestsFromTablesAppointmentIdAsync(int tablesAppointmentId);
    }
}