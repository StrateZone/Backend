using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Data;
using System.Linq;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static StrateZone_Repository.Parameters.PostgreEnums;

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
                var statusParam = parameters.Status.HasValue
                ? new NpgsqlParameter("@st", parameters.Status.Value.ToString()) { NpgsqlDbType = NpgsqlDbType.Text }
                : new NpgsqlParameter("@st", DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Text };

                IQueryable<Appointment> result = _context.Appointments
                                    .FromSqlRaw(
                                        "SELECT * FROM appointments WHERE (@st IS NULL OR status = @st::appointment_status)",
                                        statusParam)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.GameType)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.Room)
                                    .AsQueryable();

                result = parameters.OrderBy switch
                {
                    "created-at" => result.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => result.OrderByDescending(a => a.CreatedAt),
                    "total-price" => result.OrderBy(a => a.TotalPrice),
                    "total-price-desc" => result.OrderByDescending(a => a.TotalPrice),
                    "tables-count" => result.OrderBy(a => a.TablesAppointments.Count),
                    "tables-count-desc" => result.OrderByDescending(a => a.TablesAppointments.Count),
                    _ => result
                };

                return await PagedList<Appointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointment>> GetAllAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                AppointmentStatus? status = parameters.Status;

                var statusParam = status.HasValue 
                    ? new NpgsqlParameter("@st", status.Value.ToString()) { NpgsqlDbType = NpgsqlDbType.Text } 
                    : new NpgsqlParameter("@st", DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Text };

                var result = _context.Appointments
                                    .FromSqlRaw(
                                        @"SELECT a.*
                                            FROM appointments a
                                            WHERE (@st IS NULL OR a.status = @st::appointment_status)",
                                        statusParam
                                        ).Include(a => a.User)
                                        .Include(a => a.TablesAppointments)
                                            .ThenInclude(ta => ta.Table)
                                                .ThenInclude(t => t.GameType)
                                        .Include(a => a.TablesAppointments)
                                            .ThenInclude(ta => ta.Table)
                                                .ThenInclude(t => t.Room)
                                        .AsQueryable();
                
                result = parameters.OrderBy switch
                {
                    "created-at" => result.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => result.OrderByDescending(a => a.CreatedAt),
                    "total-price" => result.OrderBy(a => a.TotalPrice),
                    "total-price-desc" => result.OrderByDescending(a => a.TotalPrice),
                    "tables-count" => result.OrderBy(a => a.TablesAppointments.Count),
                    "tables-count-desc" => result.OrderByDescending(a => a.TablesAppointments.Count),
                    _ => result
                };

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
                AppointmentStatus? status = parameters.Status;

                var statusParam = status.HasValue
                    ? new NpgsqlParameter("@st", status.Value.ToString()) { NpgsqlDbType = NpgsqlDbType.Text }
                    : new NpgsqlParameter("@st", DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Text };

                var userId = new NpgsqlParameter("@id", id);

                IQueryable<Appointment> result = _context.Appointments
                                    .FromSqlRaw(
                                        "SELECT * FROM appointments " +
                                        "WHERE user_id = @id " +
                                        "AND (@st IS NULL OR status = @st::appointment_status)", 
                                        statusParam,
                                        userId)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.GameType)
                                    .Include(a => a.TablesAppointments)
                                        .ThenInclude(ta => ta.Table)
                                            .ThenInclude(t => t.Room)
                                    .AsQueryable();

                result = parameters.OrderBy switch
                {
                    "created-at" => result.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => result.OrderByDescending(a => a.CreatedAt),
                    "total-price" => result.OrderBy(a => a.TotalPrice),
                    "total-price-desc" => result.OrderByDescending(a => a.TotalPrice),
                    "tables-count" => result.OrderBy(a => a.TablesAppointments.Count),
                    "tables-count-desc" => result.OrderByDescending(a => a.TablesAppointments.Count),
                    _ => result
                };

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
                var existingAppointment = await _context.Appointments.FindAsync(id) ?? throw new Exception("Appointment with this ID does not exist");
                _context.Entry(existingAppointment).State = EntityState.Detached;

                appointment.AppointmentId = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE appointments SET ");

                if (appointment.UserId > 0)
                {
                    sql.Append("user_id = @user_id, ");
                    parameters.Add(new NpgsqlParameter("@user_id", appointment.UserId));
                }

                sql.Append("status = @status::appointment_status, ");
                parameters.Add(new NpgsqlParameter("@status", appointment.Status.ToString()));

                if (appointment.TotalPrice > 0)
                {
                    sql.Append("total_price = @total_price, ");
                    parameters.Add(new NpgsqlParameter("@total_price", appointment.TotalPrice));
                }

                if (appointment.CreatedAt.HasValue)
                {
                    sql.Append("created_at = @created_at, ");
                    parameters.Add(new NpgsqlParameter("@created_at", appointment.CreatedAt.Value));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE appointment_id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(appointment).State = EntityState.Detached;

                return await _context.Appointments.FindAsync(id);
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

        public async Task<List<Appointment>> UpdateStatusForAppointmentBasedOnTablesAppointments()
        {
            try
            {
                throw new NotImplementedException();

                DateTime CurrentTime = DateTime.UtcNow.AddHours(7);

                var result = _context.Appointments
                                .FromSqlRaw(
                                    "UPDATE tables_appointments SET status = 'expired' " +
                                    "WHERE status = 'confirmed' OR status = 'pending' OR status = 'unpaid' AND schedule_time < {0}",
                                    CurrentTime
                                    )
                                    .Include(a => a.User)
                                    .Include(a => a.TablesAppointments)
                                .ToListAsync();
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
