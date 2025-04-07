using Azure.Core;
using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Any;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Linq;
using System.Text;

namespace StrateZone_Repository.Implements
{
    public class TablesAppointmentRepository : ITablesAppointmentRepository
    {
        private readonly StrateZoneDbContext _context;

        public TablesAppointmentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<TablesAppointment>> GetAllTablesAppointmentAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = _context.TablesAppointments
                                     .Include(ta => ta.Table)
                                        .ThenInclude(t => t.Room)
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .AsQueryable();

                return await PagedList<TablesAppointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> GetByIdAsync(int id)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Include(ta => ta.Table)
                                    .SingleOrDefaultAsync(ta => ta.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<PagedList<TablesAppointment>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = _context.TablesAppointments
                                    .Where(ta => ta.TableId == id)
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.Room)
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .AsQueryable();

                return await PagedList<TablesAppointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<TablesAppointment>> GetAllTablesAppointmentByAppointmentIdAsync(int id)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Where(ta => ta.AppointmentId == id)
                                     .Include(ta => ta.Table)
                                        .ThenInclude(t => t.Room)
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .SingleOrDefaultAsync(ta => ta.TableId == tableId && ta.AppointmentId == appointmentId);
                                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> CreateTablesAppointmentAsync(TablesAppointment tablesAppointment)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var createCmd = connection.CreateCommand();

                createCmd.CommandText = @"
                    INSERT INTO tables_appointments (table_id, appointment_id, schedule_time, end_time, price, status, created_at) 
                    VALUES (@table_id, @appointment_id, @schedule_time, @end_time, @price, @status::appointment_status, @created_at)
                    RETURNING id;"
                ;

                createCmd.Parameters.Add(new NpgsqlParameter("@table_id", tablesAppointment.TableId));
                createCmd.Parameters.Add(new NpgsqlParameter("@appointment_id", tablesAppointment.AppointmentId));
                createCmd.Parameters.Add(new NpgsqlParameter("@schedule_time", tablesAppointment.ScheduleTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@end_time", tablesAppointment.EndTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@price", tablesAppointment.Price));
                createCmd.Parameters.Add(new NpgsqlParameter("@status", tablesAppointment.Status.ToString()));
                createCmd.Parameters.Add(new NpgsqlParameter("@created_at", tablesAppointment.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newTablesAppointmentId = await createCmd.ExecuteScalarAsync();
                tablesAppointment.Id = Convert.ToInt32(newTablesAppointmentId);

                return tablesAppointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TablesAppointment>> CreateTablesAppointmentsFromAppointmentAsync(Appointment appointment)
        {
            try
            {
                List<TablesAppointment> tablesAppointments = [.. appointment.TablesAppointments];
                List<TablesAppointment> createdList = new();

                foreach (var ta in tablesAppointments)
                {
                    createdList.Add (await CreateTablesAppointmentAsync(ta));
                }

                return createdList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> UpdateTablesAppointmentAsync(TablesAppointment tablesAppointment, int id)
        {
            try
            {
                var existingTablesAppointment = await _context.TablesAppointments.FindAsync(id) 
                    ?? throw new Exception("Tables appointment with this ID does not exist");

                tablesAppointment.Id = id;
                _context.Entry(existingTablesAppointment).State = EntityState.Detached;

                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE tables_appointments SET ");

                if (tablesAppointment.TableId.HasValue)
                {
                    sql.Append("table_id = @table_id, ");
                    parameters.Add(new NpgsqlParameter("@table_id", tablesAppointment.TableId));
                }

                if (tablesAppointment.AppointmentId.HasValue)
                {
                    sql.Append("appointment_id = @appointment_id, ");
                    parameters.Add(new NpgsqlParameter("@appointment_id", tablesAppointment.AppointmentId));
                }

                if (tablesAppointment.ScheduleTime != null)
                {
                    sql.Append("schedule_time = @schedule_time, ");
                    parameters.Add(new NpgsqlParameter("@schedule_time", tablesAppointment.ScheduleTime));
                }

                if (tablesAppointment.EndTime != null)
                {
                    sql.Append("end_time = @end_time, ");
                    parameters.Add(new NpgsqlParameter("@end_time", tablesAppointment.EndTime));
                }

                if (tablesAppointment.Price.HasValue)
                {
                    sql.Append("price = @price, ");
                    parameters.Add(new NpgsqlParameter("@price", tablesAppointment.Price));
                }

                if (tablesAppointment.CreatedAt.HasValue)
                {
                    sql.Append("created_at = @created_at, ");
                    parameters.Add(new NpgsqlParameter("@created_at", tablesAppointment.CreatedAt));
                }

                sql.Append("status = @status::appointment_status, ");
                parameters.Add(new NpgsqlParameter("@status", tablesAppointment.Status.ToString()));

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(tablesAppointment).State = EntityState.Detached;

                var updatedAppointmentRequest = await _context.TablesAppointments.FindAsync(id);
                return updatedAppointmentRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> DeleteTablesAppointmentAsync(int id)
        {
            try
            {
                var toDelete = await _context.TablesAppointments.FindAsync(id) ?? throw new Exception("No tables_appointment with this ID was found.");

                _context.TablesAppointments.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TablesAppointment>> GetAllTablesAppointmentsInvitedToUserByUserId(int userId, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = (from ta in _context.TablesAppointments
                             join ar in _context.AppointmentRequests
                             on new { ta.TableId, ta.AppointmentId } equals new { TableId = (int?) ar.TableId, AppointmentId = ar.AppointmentId }
                             where ar.ToUser == userId && ar.Status == PostgreEnums.RequestStatus.accepted
                             orderby ta.ScheduleTime descending
                             select ta)
                             .Include(ta => ta.Table)
                                .ThenInclude(t => t.Room)
                            .Include(ta => ta.Table)
                                .ThenInclude(t => t.GameType);

                return await PagedList<TablesAppointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TablesAppointment>> GetAllTablesAppointmentsFromUserByUserId(int userId, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = _context.TablesAppointments
                        .Where(ta => ta.Appointment.UserId == userId)
                        .Include(ta => ta.Table)
                            .ThenInclude(t => t.Room)
                        .Include(ta => ta.Table)
                            .ThenInclude(t => t.GameType)
                        .OrderByDescending(ta => ta.ScheduleTime)
                        .AsQueryable();

                return await PagedList<TablesAppointment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateStatusForExpiredAndIncomingTablesAppointments()
        {
            try
            {
                DateTime CurrentTime = DateTime.UtcNow.AddHours(7);

                var result = await _context.Database.ExecuteSqlRawAsync(
                    @"
                        UPDATE tables_appointments
                        SET status = CASE
                            WHEN status IN ('expired', 'completed', 'cancelled', 'refunded') THEN status
                            WHEN end_time < {0} AND status NOT IN ('checked_in', 'completed', 'cancelled', 'refunded') THEN 'expired'
                            WHEN end_time < {0} AND status = 'checked_in' THEN 'completed'
                            WHEN schedule_time <= {0} + INTERVAL '1.5 hours' AND status NOT IN ('checked_in', 'completed', 'cancelled', 'refunded') AND schedule_time > {0} THEN 'incoming'
                            ELSE status
                        END;
                        ",
                    CurrentTime
                );

                return result; // Returns the number of rows affected
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TablesAppointment>> GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests()
        {
            try
            {
                var matchingTAs = await _context.TablesAppointments
                            .FromSqlRaw(
                                @"SELECT ta.*
                                FROM tables_appointments ta
                                WHERE ta.status IN ('confirmed', 'pending') 
                                AND EXISTS (
                                    SELECT 1
                                    FROM appointment_requests ar
                                    WHERE ar.appointment_id = ta.appointment_id
                                      AND ar.table_id = ta.table_id
                                      AND ar.status IN ('rejected', 'expired')
                                );"
                            ).ToListAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
