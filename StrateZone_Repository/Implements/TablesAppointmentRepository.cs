using Azure.Core;
using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Any;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Globalization;
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
                                    .AsNoTracking()
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
                                    .AsNoTracking()
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
                                    .AsNoTracking()
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
                                    .AsNoTracking()
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
                return await _context.TablesAppointments.AsNoTracking()
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .FirstOrDefaultAsync(ta => ta.TableId == tableId && ta.AppointmentId == appointmentId);
                                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _context.TablesAppointments.AsNoTracking()
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .SingleOrDefaultAsync(ta => ta.TableId == tableId && ta.AppointmentId == appointmentId && ta.ScheduleTime == startTime && ta.EndTime == endTime);

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
                    INSERT INTO tables_appointments (table_id, appointment_id, schedule_time, end_time, price, status, created_at, schedule_range, paid_for_opponent, note, is_extended, extended_of_id) 
                    VALUES (@table_id, @appointment_id, @schedule_time, @end_time, @price, @status::appointment_status, @created_at, tstzrange(@schedule_time, @end_time), @paid_for_opponent, @note, @is_extended, @extended_of_id)
                    RETURNING id;"
                ;

                createCmd.Parameters.Add(new NpgsqlParameter("@table_id", tablesAppointment.TableId));
                createCmd.Parameters.Add(new NpgsqlParameter("@appointment_id", tablesAppointment.AppointmentId));
                createCmd.Parameters.Add(new NpgsqlParameter("@schedule_time", tablesAppointment.ScheduleTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@end_time", tablesAppointment.EndTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@price", tablesAppointment.Price));
                createCmd.Parameters.Add(new NpgsqlParameter("@status", tablesAppointment.Status.ToString()));
                createCmd.Parameters.Add(new NpgsqlParameter("@created_at", tablesAppointment.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));
                createCmd.Parameters.Add(new NpgsqlParameter("@paid_for_opponent", tablesAppointment.PaidForOpponent));
                createCmd.Parameters.Add(new NpgsqlParameter("@note", tablesAppointment.Note));
                createCmd.Parameters.Add(new NpgsqlParameter("@is_extended", tablesAppointment.IsExtended == null ? DBNull.Value : false));
                createCmd.Parameters.Add(new NpgsqlParameter("@extended_of_id", tablesAppointment.ExtendedOf == null ? DBNull.Value : tablesAppointment.ExtendedOf));

                var newTablesAppointmentId = await createCmd.ExecuteScalarAsync();
                tablesAppointment.Id = Convert.ToInt32(newTablesAppointmentId);

                return tablesAppointment;
            }
            catch (PostgresException ex) when (ex.SqlState == "23P01")
            {
                throw new Exception($"This table is being booked by someone else: Table ID {tablesAppointment.TableId}, schedule time: {tablesAppointment.ScheduleTime}, end time: {tablesAppointment.EndTime}");
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
                var existingTablesAppointment = await _context.TablesAppointments.AsNoTracking().SingleOrDefaultAsync(ta => ta.Id == id) 
                    ?? throw new Exception("Tables appointment with this ID does not exist");

                tablesAppointment.Id = id;

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

                if (!string.IsNullOrEmpty(tablesAppointment.Note))
                {
                    sql.Append("note = @note, ");
                    parameters.Add(new NpgsqlParameter("@note", tablesAppointment.Note));
                }

                if (tablesAppointment.IsExtended != null)
                {
                    sql.Append("paid_for_opponent = @paid_for_opponent, ");
                    parameters.Add(new NpgsqlParameter("@paid_for_opponent", tablesAppointment.PaidForOpponent));
                }

                if (tablesAppointment.ExtendedOf.HasValue)
                {
                    sql.Append("extended_of_id = @extended_of_id, ");
                    parameters.Add(new NpgsqlParameter("@extended_of_id", tablesAppointment.ExtendedOf));
                }

                if (tablesAppointment.IsExtended != null)
                {
                    sql.Append("is_extended = @is_extended, ");
                    parameters.Add(new NpgsqlParameter("@is_extended", tablesAppointment.ExtendedOf));
                }

                sql.Append("status = @status::appointment_status, ");
                parameters.Add(new NpgsqlParameter("@status", tablesAppointment.Status.ToString()));

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(tablesAppointment).State = EntityState.Detached;

                return tablesAppointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task MassUpdateTablesAppointmentsAsync(List<TablesAppointment> tablesAppointments)
        {
            if (tablesAppointments == null || !tablesAppointments.Any())
                return;

            try
            {
                var commands = new List<string>();
                var allParameters = new List<NpgsqlParameter>();

                int index = 0;

                foreach (var ta in tablesAppointments)
                {
                    var sql = new StringBuilder("UPDATE tables_appointments SET ");
                    var parameters = new List<NpgsqlParameter>();

                    if (ta.TableId.HasValue)
                    {
                        sql.Append($"table_id = @table_id_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@table_id_{index}", ta.TableId));
                    }

                    if (ta.AppointmentId.HasValue)
                    {
                        sql.Append($"appointment_id = @appointment_id_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@appointment_id_{index}", ta.AppointmentId));
                    }

                    if (ta.ScheduleTime != null)
                    {
                        sql.Append($"schedule_time = @schedule_time_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@schedule_time_{index}", ta.ScheduleTime));
                    }

                    if (ta.EndTime != null)
                    {
                        sql.Append($"end_time = @end_time_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@end_time_{index}", ta.EndTime));
                    }

                    if (ta.Price.HasValue)
                    {
                        sql.Append($"price = @price_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@price_{index}", ta.Price));
                    }

                    if (ta.CreatedAt.HasValue)
                    {
                        sql.Append($"created_at = @created_at_{index}, ");
                        parameters.Add(new NpgsqlParameter($"@created_at_{index}", ta.CreatedAt));
                    }

                    sql.Append($"status = @status_{index}::appointment_status, ");
                    parameters.Add(new NpgsqlParameter($"@status_{index}", ta.Status.ToString()));

                    sql.Remove(sql.Length - 2, 2);
                    sql.Append($" WHERE id = @id_{index}");
                    parameters.Add(new NpgsqlParameter($"@id_{index}", ta.Id));

                    commands.Add(sql.ToString());
                    allParameters.AddRange(parameters);

                    index++;
                }

                var finalSql = string.Join(";", commands);

                await _context.Database.ExecuteSqlRawAsync(finalSql, allParameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed mass update: {ex.Message}", ex);
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
                var result = _context.TablesAppointments
                                    .AsNoTracking()
                                    .Join(_context.AppointmentRequests,
                                        ta => new { ta.TableId, ta.AppointmentId },
                                        ar => new { TableId = (int?)ar.TableId, ar.AppointmentId },
                                        (ta, ar) => new { ta, ar })
                                    .Where(x => x.ar.ToUser == userId && x.ar.Status == PostgreEnums.RequestStatus.accepted)
                                    .Join(_context.Payments,
                                        x => x.ta.Id, 
                                        p => p.TablesAppointmentId,   
                                        (x, p) => new { x.ta, p })
                                        .Where(x => x.p.UserId == userId && x.p.PaymentStatus == PostgreEnums.PaymentStatus.paid)
                                        .Select(x => x.ta)
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
                        .AsNoTracking()
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

        public async Task<List<TablesAppointment>> GetAllActiveTablesAppointmentByTableIdAsync(int tableId)
        {
            try
            {
                var matchingTAs = await _context.TablesAppointments
                                            .FromSqlRaw(
                                                @"
                                                SELECT *
                                                FROM tables_appointments
                                                WHERE table_id = {0} AND status IN ('confirmed', 'pending', 'incoming');",
                                                tableId
                                            )
                                            .AsNoTracking()
                                            .ToListAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberOfAllActiveTablesAppointmentByTableIdAsync(int tableId)
        {
            try
            {
                var result = await _context.Database
                            .SqlQuery<int?>(
                                $@"
                                SELECT COUNT(*) AS ""Value""
                                FROM tables_appointments
                                WHERE table_id = {tableId} AND status IN ('confirmed', 'pending', 'incoming')"
                            )
                            .FirstOrDefaultAsync();

                return result ?? 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<TablesAppointment>> GetAllActiveTablesAppointmentByRoomIdAsync(int roomId)
        {
            try
            {
                var matchingTAs = await _context.TablesAppointments
                                            .FromSqlRaw(
                                                @"
                                                SELECT ta.*
                                                FROM tables_appointments ta
                                                JOIN tables t ON t.table_id = ta.table_id
                                                JOIN rooms r ON r.room_id = t.room_id
                                                WHERE r.room_id = {0} AND ta.status IN ('confirmed', 'pending', 'incoming');",
                                                roomId
                                            )
                                            .AsNoTracking()
                                            .ToListAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberOfAllActiveTablesAppointmentByRoomIdAsync(int roomId)
        {
            try
            {
                var result = await _context.Database
                            .SqlQuery<int?>(
                                $@"
                                SELECT COUNT(ta.*) AS ""Value""
                                FROM tables_appointments ta
                                JOIN tables t ON t.table_id = ta.table_id
                                JOIN rooms r ON r.room_id = t.room_id
                                WHERE r.room_id = {roomId} AND ta.status IN ('confirmed', 'pending', 'incoming')"
                            )
                            .FirstOrDefaultAsync();

                return result ?? 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<TablesAppointment>> GetAllActiveTablesAppointmentByGameTypeIdAsync(int typeId)
        {
            try
            {
                var matchingTAs = await _context.TablesAppointments
                                            .FromSqlRaw(
                                                @"
                                                SELECT ta.*
                                                FROM tables_appointments ta
                                                JOIN tables t ON t.table_id = ta.table_id
                                                WHERE t.""gameType_id"" = {0} AND ta.status IN ('confirmed', 'pending', 'incoming');",
                                                typeId
                                            )
                                            .AsNoTracking()
                                            .ToListAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberOfAllActiveTablesAppointmentByGametypeIdAsync(int roomId)
        {
            try
            {
                var result = await _context.Database
                            .SqlQuery<int?>(
                                $@"
                                SELECT COUNT(ta.*) AS ""Value"" 
                                FROM tables_appointments ta 
                                JOIN tables t ON t.table_id = ta.table_id
                                WHERE t.""gameType_id"" = {roomId} AND ta.status IN ('confirmed', 'pending', 'incoming')"
                            )
                            .FirstOrDefaultAsync();

                return result ?? 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateStatusForExpiredAndIncomingTablesAppointments()
        {
            try
            {
                DateTime CurrentTime = DateTime.UtcNow.AddHours(7);

                decimal incomingTime = (await _context.Systems.AsNoTracking().SingleOrDefaultAsync(s => s.Id == 1)).Appointment_Incoming_HoursFromScheduleTime;

                var result = await _context.Database.ExecuteSqlRawAsync(
                    @"
                        UPDATE tables_appointments
                        SET status = CASE
                            WHEN status IN ('expired', 'completed', 'cancelled', 'refunded') THEN status
                            WHEN end_time < {0} AND status NOT IN ('checked_in', 'completed', 'cancelled', 'refunded') THEN 'expired'
                            WHEN end_time < {0} AND status = 'checked_in' THEN 'completed'
                            WHEN schedule_time <= {0} + ({1} || ' hours')::interval
                                 AND status NOT IN ('checked_in', 'completed', 'cancelled', 'refunded') 
                                 AND schedule_time > {0} THEN 'incoming'
                            ELSE status
                        END;
                        ",
                    CurrentTime,
                    incomingTime.ToString(CultureInfo.InvariantCulture)
                );


                return result;
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
                                    @"
                                    SELECT ta.*
                                    FROM tables_appointments ta
                                    WHERE ta.status IN ('confirmed', 'pending', 'incoming')
                                      AND (
                                          EXISTS (
                                              SELECT 1
                                              FROM appointment_requests ar
                                              WHERE ar.appointment_id = ta.appointment_id
                                                AND ar.table_id = ta.table_id
                                          )
                                          AND NOT EXISTS (
                                              SELECT 1
                                              FROM appointment_requests ar
                                              WHERE ar.appointment_id = ta.appointment_id
                                                AND ar.table_id = ta.table_id
                                                AND ar.status NOT IN ('expired', 'cancelled', 'rejected')
                                          )
                                      );
                                    "
                            ).AsNoTracking().ToListAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberOfTablesAppointmentCancelledByUserInAWeekSpanAsync(int userId, DateTime currentDate)
        {
            try
            {
                DateTime monday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (currentDate.DayOfWeek == DayOfWeek.Sunday ? -6 : 1)).Date;

                var matchingTAs = await _context.TablesAppointments
                            .FromSqlRaw(
                                    @"
                                        SELECT ta.*
                                        FROM tables_appointments ta
                                        JOIN appointments a ON a.appointment_id = ta.appointment_id
                                        WHERE a.user_id = @user_id 
                                        AND NOT EXISTS (SELECT 1 FROM appointment_requests ar WHERE ar.table_id = ta.table_id AND ar.appointment_id = ta.appointment_id AND ar.status = 'table_cancelled')
                                        AND ta.status = 'cancelled' 
                                        AND ta.created_at >= @monday AND ta.created_at <= @today
                                    ",
                                    new NpgsqlParameter("@monday", monday),
                                    new NpgsqlParameter("@today", currentDate),
                                    new NpgsqlParameter("@user_id", userId)
                            )
                            .AsNoTracking()
                            .CountAsync();

                return matchingTAs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetSumOfPaidTablesAppointmentWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                var totalPrice = await _context.Database
                    .SqlQuery<decimal?>(
                        $@"
                            SELECT SUM(
                                CASE 
                                    WHEN sub.paid_count >= 2 THEN ta.price * 2
                                    ELSE ta.price
                                END
                            ), EXTRACT(DAY FROM ta.created_at) AS ""Value""
                            FROM tables_appointments ta
                            JOIN (
                                SELECT tables_appointment_id, COUNT(*) AS paid_count
                                FROM payments
                                WHERE status = 'paid'
                                GROUP BY tables_appointment_id
                            ) sub ON sub.tables_appointment_id = ta.id
                            WHERE EXTRACT(YEAR FROM ta.created_at) = {year} AND EXTRACT(MONTH FROM ta.created_at) = {month}
                        ")
                    .FirstOrDefaultAsync();

                return totalPrice ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetAllPaidTablesAppointmentWithinADayInYearAsync(int day, int month, int year)
        {
            try
            {
                var totalPrice = await _context.Database
                    .SqlQuery<decimal?>(
                        $@"
                            SELECT SUM(
                                CASE 
                                    WHEN sub.paid_count >= 2 THEN ta.price * 2
                                    ELSE ta.price
                                END
                            ) AS ""Value""
                            FROM tables_appointments ta
                            JOIN (
                                SELECT tables_appointment_id, COUNT(*) AS paid_count
                                FROM payments
                                WHERE status = 'paid'
                                GROUP BY tables_appointment_id
                            ) sub ON sub.tables_appointment_id = ta.id
                            WHERE EXTRACT(YEAR FROM ta.created_at) = {year} AND EXTRACT(MONTH FROM ta.created_at) = {month} AND EXTRACT(DAY FROM ta.created_at) = {day}
                        ")
                    .FirstOrDefaultAsync();

                return totalPrice ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointment>> GetAllBookedTablesAppointmentWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                var total = await _context.TablesAppointments.AsNoTracking()
                                        .Where(u => u.CreatedAt.HasValue
                                            && u.CreatedAt.Value.Year == year
                                            && u.CreatedAt.Value.Month == month
                                            )
                                        .ToListAsync();

                return total;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetCountAllBookedTablesAppointmentWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                return await _context.TablesAppointments.AsNoTracking()
                                        .Where(u => u.CreatedAt.HasValue
                                            && u.CreatedAt.Value.Year == year
                                            && u.CreatedAt.Value.Month == month
                                            )
                                        .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
