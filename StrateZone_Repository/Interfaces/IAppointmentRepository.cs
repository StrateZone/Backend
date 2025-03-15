using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Implements
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> DeleteAppointmentAsync(int id);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<PagedList<Appointment>> GetAppointmentsAsync(AppointmentParameters parameters);
        Task<PagedList<Appointment>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int id);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment, int id);
    }
}