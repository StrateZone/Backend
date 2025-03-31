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

        public AppointmentRepository(StrateZoneDbContext context)
        {
            _context = context;
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
                                    .OrderByDescending(a => a.CreatedAt)
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

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO appointments (user_id, total_price, status, created_at) 
                    VALUES (@user_id, @total_price, @status::appointment_status, @created_at)
                    RETURNING appointment_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@user_id", appointment.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("@total_price", appointment.TotalPrice));
                cmd.Parameters.Add(new NpgsqlParameter("@status", appointment.Status.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@created_at", appointment.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

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
