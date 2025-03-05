using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Implements
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> DeleteAppointmentAsync(int id);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<List<Appointment>> GetAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment, int id);
    }
}