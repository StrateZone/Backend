using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface ITablesAppointmentService
    {
        Task<TablesAppointmentModel> CreateTablesAppointmentAsync(TablesAppointmentModel tablesAppointmentModel);
        Task<List<TablesAppointmentModel>> CreateTablesAppointmentsFromAppointmentAsync(AppointmentModel appointmentModel);
        Task<TablesAppointmentModel> UpdateTablesAppointmentAsync(TablesAppointmentModel appointmentModel, int id);
        Task<TablesAppointmentModel> DeleteTablesAppointmentAsync(int id);
        Task<PagedList<TablesAppointmentModel>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters);
        Task<List<TablesAppointmentModel>> GetAllTablesAppointmentByAppointmentIdAsync(int id);
        Task<PagedList<TablesAppointmentModel>> GetAllTablesAppointmentsByUserId(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TablesAppointmentModel>> GetAllTablesAppointmentsJoinedByUserId(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TablesAppointmentModel>> GetAllTablesAppointmentsAsync(TablesAppointmentParameters parameters);
        Task<TablesAppointmentModel> GetByIdAsync(int id);
        Task<TablesAppointmentModel> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId);
        Task<TablesAppointmentModel> CheckInTablesAppointment(int tablesAppointmentId, int userId);
        Task<TablesAppointmentModel> CancelTablesAppointment(int tablesAppointmentId, int userId);
    }
}