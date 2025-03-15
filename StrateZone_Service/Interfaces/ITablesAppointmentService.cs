using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface ITablesAppointmentService
    {
        Task<TablesAppointmentModel> CreateTablesAppointment(TablesAppointmentModel tablesAppointmentModel);
        Task<List<TablesAppointmentModel>> CreateTablesAppointmentsFromAppointmentAsync(AppointmentModel appointmentModel);
        Task<TablesAppointmentModel> DeleteTablesAppointmentAsync(int id);
        Task<List<TablesAppointmentModel>> GetAllTablesAppointmentByTableIdAsync(int id);
        Task<List<TablesAppointmentModel>> GetAllTablesAppointmentByAppointmentIdAsync(int id);
        Task<List<TablesAppointmentModel>> GetAllTablesAppointmentsAsync();
        Task<TablesAppointmentModel> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId);
    }
}