using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IAppointmentrequestService
    {
        Task<AppointmentrequestModel> CreateAppointmentRequestAsync(AppointmentrequestRequest appointmentRequestModel);
        Task<AppointmentrequestModel> DeleteAppointmentRequestAsync(int id);
        Task<AppointmentrequestModel> GetAppointmentRequestByIdAsync(int id);
        Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId);
        Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId);
        Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsByAppointmnetIdAsync(AppointmentRequestParameters parameters, int appointmentId);
        Task<AppointmentrequestModel> UpdateAppointmentRequestAsync(AppointmentrequestModel appointmentRequestModel, int id);
    }
}
