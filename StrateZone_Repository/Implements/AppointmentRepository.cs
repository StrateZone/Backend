using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Data;

namespace StrateZone_Repository.Implements
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly StrateZoneDbContext _context;
        private readonly IPriceRepository _priceRepository;

        public AppointmentRepository(StrateZoneDbContext context, IPriceRepository priceRepository)
        {
            _context = context;
            _priceRepository = priceRepository;
        }

        public async Task<PagedList<Appointment>> GetAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                var result = _context.Appointments
                                    .Include(a => a.User)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.GameType)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.Room)
                                    .AsQueryable();
                return await PagedList<Appointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _context.Appointments
                    .Where(a => a.AppointmentId == id)
                    .Include(a => a.TablesAppointments)
                        .ThenInclude(ta => ta.Table)
                            .ThenInclude(t => t.GameType)
                    .Include(a => a.TablesAppointments)
                        .ThenInclude(ta => ta.Table)
                            .ThenInclude(t => t.Room)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointment>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int id)
        {
            try
            {
                var result = _context.Appointments
                                    .Where(a => a.UserId == id)
                                    .Include(a => a.User)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.GameType)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.Room)
                                    .AsQueryable();

                return await PagedList<Appointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO appointments (schedule_time, end_time, user_id, total_price, status, created_at) 
                    VALUES (@schedule_time, @end_time, @user_id, @total_price, @status::appointment_status, @created_at)
                    RETURNING appointment_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@schedule_time", appointment.ScheduleTime));
                cmd.Parameters.Add(new NpgsqlParameter("@end_time", appointment.EndTime));
                cmd.Parameters.Add(new NpgsqlParameter("@user_id", appointment.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("@total_price", appointment.TotalPrice));
                cmd.Parameters.Add(new NpgsqlParameter("@status", appointment.Status.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@created_at", appointment.CreatedAt ?? DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)));

                var newAppointmentId = await cmd.ExecuteScalarAsync();
                int appointmentId = Convert.ToInt32(newAppointmentId);

                appointment.AppointmentId = appointmentId;

                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating appointment: {ex.Message}");
            }
        }


        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment, int id)
        {
            try
            {
                if (await _context.Appointments.FindAsync(id) == null) throw new Exception("Appointment with this ID does not exist");

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();

                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            try
            {
                Appointment toRemove = await _context.Appointments.FindAsync(id) ?? throw new Exception("Appointment with this ID does not exist");

                _context.Appointments.Remove(toRemove);
                await _context.SaveChangesAsync();

                return toRemove;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
