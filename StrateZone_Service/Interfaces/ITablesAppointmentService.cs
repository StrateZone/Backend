using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface ITablesAppointmentService
    {
        Task<TablesAppointmentModel> CreateTablesAppointmentAsync(TablesAppointmentModel tablesAppointmentModel);
        Task<string> CreateCheckinQrCode(int userId, int tablesAppointmentId);
        Task<List<TablesAppointmentModel>> CreateTablesAppointmentsFromAppointmentAsync(AppointmentModel appointmentModel);
        Task<TablesAppointmentModel> UpdateTablesAppointmentAsync(TablesAppointmentModel appointmentModel, int id);
        Task<TablesAppointmentModel> DeleteTablesAppointmentAsync(int id);
        Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters);
        Task<List<TablesAppointmentResponse>> GetAllTablesAppointmentByAppointmentIdAsync(int id);
        Task<decimal> GetTotalPriceOfPaidTablesAppointmentWithinAMonthOfYearAsync(int month, int year);
        Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsByUserId(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsJoinedByUserId(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsAsync(TablesAppointmentParameters parameters);
        Task<TablesAppointmentResponse> GetByIdAsync(int id);
        Task<TablesAppointmentResponse> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId);
        Task<TablesAppointmentModel> CheckInTablesAppointment(int tablesAppointmentId, int userId);
        Task<TablesAppointmentModel> CheckoutTablesAppointment(int tablesAppointmentId, int userId);
        Task<TablesAppointmentModel> CancelTablesAppointment(int tablesAppointmentId, int userId);
        Task<TablesAppointmentModel> ForceCancelTablesAppointment(int tablesAppointmentId, int userId);
        Task<TablesAppointmentRefundResponse> CalculateRefundAmountOnAppointmentCancellation(int userId, int tablesAppointmentId, DateTime CancelTime);
        Task<List<TablesAppointmentModel>> GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests();
        Task<int> UpdateStatusForExpiredAndIncomingTablesAppointments();
        Task<TablesAppointmentsMonthResponse> GetAllBookedTablesAppointmentWithinAMonthInYearAsync(int month, int year);
    }
}