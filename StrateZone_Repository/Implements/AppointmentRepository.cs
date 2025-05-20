using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System.Data;
using System.Text;
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
                                    .AsNoTracking();

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

        public async Task<PagedList<Appointment>> GetAllAppointmentsAsync(AppointmentAdminParameters parameters)
        {
            try
            {
                AppointmentStatus? status = parameters.Status;

                var statusParam = status.HasValue 
                    ? new NpgsqlParameter("@st", status.Value.ToString()) { NpgsqlDbType = NpgsqlDbType.Text } 
                    : new NpgsqlParameter("@st", DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Text };

                var result = _context.Appointments
                                            .FromSqlRaw(@"
                                                SELECT DISTINCT a.* 
                                                FROM appointments a 
                                                JOIN tables_appointments ta 
                                                ON ta.appointment_id = a.appointment_id 
                                                AND (@st IS NULL OR ta.status = @st::appointment_status) 
                                            ", statusParam)
                                            .AsNoTracking()
                                            .Include(a => a.User)
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

                if (!string.IsNullOrWhiteSpace(parameters.SearchValue))
                {
                    string search = parameters.SearchValue.Trim().ToLower();

                    result = result.Where(a =>
                        a.AppointmentId.ToString().ToLower().Contains(search) ||
                        a.User.Email.ToLower().Contains(search));
                }

                return await PagedList<Appointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointment>> GetAllAppointmentsCheckinAsync(AppointmentAdminParameters parameters)
        {
            try
            {
                AppointmentStatus? status = parameters.Status;

                var statusParam = status.HasValue
                    ? new NpgsqlParameter("@st", status.Value.ToString()) { NpgsqlDbType = NpgsqlDbType.Text }
                    : new NpgsqlParameter("@st", DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Text };
                var today = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7).Date, DateTimeKind.Unspecified);
                var dateParam = new NpgsqlParameter("@today", today) { NpgsqlDbType = NpgsqlDbType.Timestamp };

                var result = _context.Appointments
                                    .FromSqlRaw(
                                        @"SELECT DISTINCT a.*
                                          FROM appointments a
                                          JOIN tables_appointments ta ON ta.appointment_id = a.appointment_id
                                          WHERE (@st IS NULL OR a.status = @st::appointment_status)
                                            AND ta.schedule_time >= @today AND ta.schedule_time < @today + interval '1 day'",
                                        statusParam, dateParam
                                        )
                                        .AsNoTracking()
                                        .Include(a => a.User)
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

                if (!string.IsNullOrWhiteSpace(parameters.SearchValue))
                {
                    string search = parameters.SearchValue.Trim().ToLower();

                    result = result.Where(a =>
                        a.AppointmentId.ToString().ToLower().Contains(search) ||
                        a.User.Email.ToLower().Contains(search));
                }


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
                    .AsNoTracking()
                    .Include(a => a.TablesAppointments)
                        .ThenInclude(ta => ta.Table)
                            .ThenInclude(t => t.GameType)
                    .Include(a => a.TablesAppointments)
                        .ThenInclude(ta => ta.Table)
                            .ThenInclude(t => t.Room)
                    .SingleOrDefaultAsync(a => a.AppointmentId == id);
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
                                    .AsNoTracking()
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

        public async Task<int> GetTablesCountForAppointment(int id)
        {
            try
            {
                return await _context.TablesAppointments.AsNoTracking().CountAsync(ta => ta.AppointmentId == id);
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
                var existingAppointment = await _context.Appointments.AsNoTracking().SingleOrDefaultAsync(a => a.AppointmentId == id) ?? throw new Exception("Appointment with this ID does not exist");

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

                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> UpdateAppointmentPriceAsync(int id)
        {
            try
            {
                Appointment ToUpdate = await _context.Appointments.Include(a => a.TablesAppointments).FirstOrDefaultAsync(a => a.AppointmentId == id) ?? throw new Exception("Appointment with this ID does not exist");

                ToUpdate.TotalPrice = (decimal)ToUpdate.TablesAppointments.Sum(ta => ta.Price);

                await UpdateAppointmentAsync(ToUpdate, id);

                return ToUpdate;
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

        public async Task<List<Appointment>> GetAppointmentsWithIncompletedStatusToBeCompletedBasedOnTablesAppointments()
        {
            try
            {
                var result = await _context.Appointments
                                .FromSqlRaw(@"
                                    SELECT * FROM appointments a
                                    WHERE status = 'incompleted' 
                                    AND NOT EXISTS (
	                                    SELECT 1 FROM tables_appointments ta
	                                    WHERE ta.appointment_id = a.appointment_id
	                                    AND ta.status IN ('pending', 'confirmed', 'incoming', 'checked_in')
	                                )"
                                )
                                .Include(a => a.TablesAppointments)
                                .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
