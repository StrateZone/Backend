using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface ITablesAppointmentRepository
    {
        Task<TablesAppointment> CreateTablesAppointmentAsync(TablesAppointment tablesAppointment);
        Task<List<TablesAppointment>> CreateTablesAppointmentsFromAppointmentAsync(Appointment appointment);
        Task<TablesAppointment> DeleteTablesAppointmentAsync(int id);
        Task<List<TablesAppointment>> GetAllTablesAppointmentAsync();
        Task<List<TablesAppointment>> GetAllTablesAppointmentByTableIdAsync(int id);
        Task<List<TablesAppointment>> GetAllTablesAppointmentByAppointmentIdAsync(int id);
        Task<TablesAppointment> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId);
    }
}