using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface ITablesAppointmentRepository
    {
        Task<TablesAppointment> CreateTablesAppointmentAsync(TablesAppointment tablesAppointment);
        Task<List<TablesAppointment>> CreateTablesAppointmentsFromAppointmentAsync(Appointment appointment);
        Task<TablesAppointment> UpdateTablesAppointmentAsync(TablesAppointment tablesAppointment, int id);
        Task<TablesAppointment> DeleteTablesAppointmentAsync(int id);
        Task<PagedList<TablesAppointment>> GetAllTablesAppointmentAsync(TablesAppointmentParameters parameters);
        Task<TablesAppointment> GetByIdAsync(int id);
        Task<PagedList<TablesAppointment>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters);
        Task<List<TablesAppointment>> GetAllTablesAppointmentByAppointmentIdAsync(int id);
        Task<PagedList<TablesAppointment>> GetAllTablesAppointmentsFromUserByUserId(int userId, TablesAppointmentParameters parameters);
        Task<PagedList<TablesAppointment>> GetAllTablesAppointmentsInvitedToUserByUserId(int userId, TablesAppointmentParameters parameters);
        Task<TablesAppointment> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId);
        Task<int> UpdateStatusForExpiredTablesAppointments();
    }
}