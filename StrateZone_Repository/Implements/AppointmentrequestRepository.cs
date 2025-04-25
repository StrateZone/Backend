using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Reflection.Metadata;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class AppointmentrequestRepository : IAppointmentrequestRepository
    {
        private readonly StrateZoneDbContext _context;
        
        public AppointmentrequestRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Appointmentrequest>> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = _context.AppointmentRequests           
                                    .AsNoTracking()
                                    .Where(ar => ar.ToUser == userId)
                                    .Include(ar => ar.FromUserNavigation)
                                    .Include(ar => ar.Appointment)
                                        .ThenInclude(ar => ar.TablesAppointments)
                                    .Include(ar => ar.Table)
                                    .ThenInclude(t => t.Room)
                                    .OrderByDescending(ar => ar.CreatedAt)
                                    .AsQueryable();

                result = parameters.OrderBy switch
                {
                    "created-at" => result.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => result.OrderByDescending(a => a.CreatedAt),
                    _ => result
                };

                return await PagedList<Appointmentrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointmentrequest>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = _context.AppointmentRequests
                                    .AsNoTracking()
                                    .Where(ar => ar.FromUser == userId)
                                    .Include(ar => ar.ToUserNavigation)
                                    .Include(ar => ar.Appointment)
                                        .ThenInclude(ar => ar.TablesAppointments)
                                    .Include(ar => ar.Table)
                                    .ThenInclude(t => t.Room)
                                    .OrderByDescending(ar => ar.CreatedAt)
                                    .AsQueryable();

                result = parameters.OrderBy switch
                {
                    "created-at" => result.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => result.OrderByDescending(a => a.CreatedAt),
                    _ => result
                };

                return await PagedList<Appointmentrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointmentrequest> GetAppointmentRequestByIdAsync(int id)
        {
            try
            {
                return await _context.AppointmentRequests
                                    .AsNoTracking()
                                    .Where(ar => ar.Id == id)
                                    .Include(ar => ar.FromUserNavigation)
                                    .Include(ar => ar.ToUserNavigation)
                                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointmentrequest> CreateAppointmentRequestAsync(Appointmentrequest appointmentRequest)
        {
            try
            {
                var requestsList = await _context.AppointmentRequests
                                                .Where(ar => 
                                                    ar.FromUser == appointmentRequest.FromUser 
                                                    && ar.TableId == appointmentRequest.TableId
                                                    && ar.StartTime == appointmentRequest.StartTime && ar.EndTime == appointmentRequest.EndTime
                                                    && (ar.AppointmentId == appointmentRequest.AppointmentId ||
                                                    (ar.AppointmentId == null && appointmentRequest.AppointmentId == null))
                                                )
                                                .ToListAsync();

                if (requestsList.Any(r => r.Status == RequestStatus.accepted))
                    throw new Exception($"Someone has already accepted your invitation to this table. Invitation is no longer allowed.");

                if (requestsList.Any(r => r.ToUser == appointmentRequest.ToUser && r.Status == PostgreEnums.RequestStatus.pending))
                    throw new Exception($"Invitation to this user already been sent.");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var createCmd = connection.CreateCommand();

                createCmd.CommandText = @"
                    INSERT INTO appointment_requests (from_user, to_user, table_id, appointment_id, estimated_price, status, start_time, end_time, expire_at, created_at) 
                    VALUES (@from_user, @to_user, @table_id, @appointment_id, @estimated_price, @status::request_status, @start_time, @end_time, @expire_at, @created_at)
                    RETURNING id;"
                ;

                createCmd.Parameters.Add(new NpgsqlParameter("@from_user", appointmentRequest.FromUser));
                createCmd.Parameters.Add(new NpgsqlParameter("@to_user", appointmentRequest.ToUser));
                createCmd.Parameters.Add(new NpgsqlParameter("@table_id", appointmentRequest.TableId));
                createCmd.Parameters.Add(new NpgsqlParameter("@appointment_id", appointmentRequest.AppointmentId == null ? DBNull.Value : appointmentRequest.AppointmentId));
                createCmd.Parameters.Add(new NpgsqlParameter("@estimated_price", appointmentRequest.TotalPrice));
                createCmd.Parameters.Add(new NpgsqlParameter("@status", appointmentRequest.Status.ToString()));
                createCmd.Parameters.Add(new NpgsqlParameter("@start_time", appointmentRequest.StartTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@end_time", appointmentRequest.EndTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@expire_at", appointmentRequest.ExpireAt));
                createCmd.Parameters.Add(new NpgsqlParameter("@created_at", appointmentRequest.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newAppointmentId = await createCmd.ExecuteScalarAsync();
                await _context.SaveChangesAsync();

                return await _context.AppointmentRequests.AsNoTracking()
                    .Include(ar => ar.FromUserNavigation)
                    .SingleOrDefaultAsync(ar => ar.Id == Convert.ToInt32(newAppointmentId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<Appointmentrequest>> CreateAppointmentRequestsAsync(List<Appointmentrequest> appointmentRequest)
        {
            try
            {
                var requestsList = await _context.AppointmentRequests
                                                .AsNoTracking()
                                                .Where(ar =>
                                                    ar.FromUser == appointmentRequest[0].FromUser
                                                    && ar.TableId == appointmentRequest[0].TableId
                                                    && ar.StartTime == appointmentRequest[0].StartTime && ar.EndTime == appointmentRequest[0].EndTime
                                                    && (ar.AppointmentId == appointmentRequest[0].AppointmentId ||
                                                    (ar.AppointmentId == null && appointmentRequest[0].AppointmentId == null))
                                                )
                                                .ToListAsync();

                if (requestsList.Any(r => r.Status == RequestStatus.accepted))
                    throw new Exception($"Someone has already accepted your invitation to this table. Invitation is no longer allowed.");

                var invitedUsers = appointmentRequest.Select(u => u.ToUser).ToHashSet();
                if (requestsList.Any(r => invitedUsers.Contains(r.ToUser) && r.Status == PostgreEnums.RequestStatus.pending))
                    throw new Exception($"Invitation to this user already been sent.");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var transaction = await connection.BeginTransactionAsync();
                await using var createCmd = connection.CreateCommand();
                createCmd.Transaction = transaction;

                createCmd.CommandText = @"
                    INSERT INTO appointment_requests (from_user, to_user, table_id, appointment_id, estimated_price, status, start_time, end_time, expire_at, created_at) 
                    VALUES (@from_user, @to_user, @table_id, @appointment_id, @estimated_price, @status::request_status, @start_time, @end_time, @expire_at, @created_at)
                    RETURNING id;"
                ;

                HashSet<int> createdIds = new();

                foreach (var request in appointmentRequest)
                {
                    createCmd.Parameters.Clear();
                    createCmd.Parameters.Add(new NpgsqlParameter("@from_user", request.FromUser));
                    createCmd.Parameters.Add(new NpgsqlParameter("@to_user", request.ToUser));
                    createCmd.Parameters.Add(new NpgsqlParameter("@table_id", request.TableId));
                    createCmd.Parameters.Add(new NpgsqlParameter("@appointment_id", request.AppointmentId == null ? DBNull.Value : request.AppointmentId));
                    createCmd.Parameters.Add(new NpgsqlParameter("@estimated_price", request.TotalPrice));
                    createCmd.Parameters.Add(new NpgsqlParameter("@status", request.Status.ToString()));
                    createCmd.Parameters.Add(new NpgsqlParameter("@start_time", request.StartTime));
                    createCmd.Parameters.Add(new NpgsqlParameter("@end_time", request.EndTime));
                    createCmd.Parameters.Add(new NpgsqlParameter("@expire_at", request.ExpireAt));
                    createCmd.Parameters.Add(new NpgsqlParameter("@created_at", request.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                    var newId = await createCmd.ExecuteScalarAsync();
                    createdIds.Add(Convert.ToInt32(newId));
                }

                await transaction.CommitAsync();
                await _context.SaveChangesAsync();

                return await _context.AppointmentRequests
                                    .AsNoTracking()
                                    .Where(ar => createdIds.Contains(ar.Id))
                                    .Include(ar => ar.FromUserNavigation)
                                    .Include(ar => ar.ToUserNavigation)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<Appointmentrequest> UpdateAppointmentRequestAsync(Appointmentrequest appointmentRequest, int id)
        {
            try
            {
                var existingAppointmentrequest = await _context.AppointmentRequests.FindAsync(id) ?? throw new Exception("Appointment request with this ID does not exist");

                _context.Entry(existingAppointmentrequest).State = EntityState.Detached;

                appointmentRequest.Id = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE appointment_requests SET ");

                if (appointmentRequest.FromUser > 0)
                {
                    sql.Append("from_user = @from_user, ");
                    parameters.Add(new NpgsqlParameter("@from_user", appointmentRequest.FromUser));
                }

                if (appointmentRequest.ToUser > 0)
                {
                    sql.Append("to_user = @to_user, ");
                    parameters.Add(new NpgsqlParameter("@to_user", appointmentRequest.ToUser));
                }

                if (appointmentRequest.TableId > 0)
                {
                    sql.Append("table_id = @table_id, ");
                    parameters.Add(new NpgsqlParameter("@table_id", appointmentRequest.TableId));
                }

                if (appointmentRequest.AppointmentId > 0)
                {
                    sql.Append("appointment_id = @appointment_id, ");
                    parameters.Add(new NpgsqlParameter("@appointment_id", appointmentRequest.AppointmentId));
                }

                sql.Append("status = @status::request_status, ");
                parameters.Add(new NpgsqlParameter("@status", appointmentRequest.Status.ToString()));

                if (appointmentRequest.StartTime.HasValue)
                {
                    sql.Append("start_time = @start_time, ");
                    parameters.Add(new NpgsqlParameter("@start_time", appointmentRequest.StartTime.Value));
                }

                if (appointmentRequest.EndTime.HasValue)
                {
                    sql.Append("end_time = @end_time, ");
                    parameters.Add(new NpgsqlParameter("@end_time", appointmentRequest.EndTime.Value));
                }

                if (appointmentRequest.ExpireAt.HasValue)
                {
                    sql.Append("expire_at = @expire_at, ");
                    parameters.Add(new NpgsqlParameter("@expire_at", appointmentRequest.ExpireAt.Value));
                }

                if (appointmentRequest.CreatedAt.HasValue)
                {
                    sql.Append("created_at = @created_at, ");
                    parameters.Add(new NpgsqlParameter("@created_at", appointmentRequest.CreatedAt.Value));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(appointmentRequest).State = EntityState.Detached;

                return await _context.AppointmentRequests.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointmentrequest> AcceptAppointmentrequestAsync(int id)
        {
            try
            {
                var toAccept = await _context.AppointmentRequests.FindAsync(id)
                            ?? throw new Exception("Appointment request with this ID does not exist");

                if (toAccept.Status != PostgreEnums.RequestStatus.pending)
                {
                    throw new Exception($"This request is already {toAccept.Status}.");
                }

                toAccept.Id = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder(
                    "UPDATE appointment_requests " +
                        "SET status = " +
                        "CASE " +
                            "WHEN id = @id THEN 'accepted' " +
                            "WHEN status = 'pending' AND id != @id AND from_user = @user_id " +
                                "AND table_id = @table_id AND (appointment_id IS NULL OR appointment_id = @appointment_id) THEN 'accepted_by_others' " +
                            "ELSE status " +
                    "END;");
                parameters.Add(new NpgsqlParameter("@id", id));
                parameters.Add(new NpgsqlParameter("@user_id", toAccept.FromUser));
                parameters.Add(new NpgsqlParameter("@table_id", toAccept.TableId));
                parameters.Add(new NpgsqlParameter("@appointment_id", toAccept.AppointmentId != null ? toAccept.AppointmentId : DBNull.Value));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(toAccept).State = EntityState.Detached;

                return await _context.AppointmentRequests.AsNoTracking().Include(ar => ar.ToUserNavigation).SingleOrDefaultAsync(ar => ar.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointmentrequest> RejectAppointmentrequestAsync(int id)
        {
            try
            {
                var toReject = await _context.AppointmentRequests.FindAsync(id)
                            ?? throw new Exception("Appointment request with this ID does not exist");

                if (toReject.Status != PostgreEnums.RequestStatus.pending)
                {
                    throw new Exception($"This request is already {toReject.Status}.");
                }

                toReject.Id = id;
                var sql = new StringBuilder("UPDATE appointment_requests SET status = 'rejected' WHERE id = @id;");

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), new NpgsqlParameter("@id", id));
                _context.Entry(toReject).State = EntityState.Detached;

                return await _context.AppointmentRequests.AsNoTracking().Include(ar => ar.ToUserNavigation).SingleOrDefaultAsync(ar => ar.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> CancelAllSentRequestFromUserAsync(int userId)
        {
            try
            {
                var updatedRequests = await _context.AppointmentRequests
                    .FromSqlRaw(
                        "UPDATE appointment_requests " +
                        "SET status = 'cancelled' " +
                        "WHERE from_user = {0} AND status != 'expired' AND appointment_id IS NULL " +
                        "RETURNING *;", userId)
                    .ToListAsync();

                return updatedRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointmentrequest> DeleteAppointmentRequestAsync(int id)
        {
            try
            {
                var toDelete = await _context.AppointmentRequests.FindAsync(id) ?? throw new Exception("Appointment request with this ID does not exist.");
                _context.AppointmentRequests.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointmentrequest>> GetAppointmentRequestsOfUserByTableIdAsync(AppointmentRequestParameters parameters, int tableId)
        {
            try
            {
                var result = _context.AppointmentRequests
                                    .Where(ar => ar.TableId == tableId)
                                    .AsQueryable();
                return await PagedList<Appointmentrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Appointmentrequest>> GetAppointmentRequestsOfUserByAppointmentAndTableIdAsync(AppointmentRequestParameters parameters, int appointmentId, int tableId)
        {
            try
            {
                var result = _context.AppointmentRequests
                                    .Where(ar => ar.AppointmentId == appointmentId && ar.TableId == tableId)
                                    .AsQueryable();
                return await PagedList<Appointmentrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> GetCurrentAppointmentRequestsFromUserByUserAndTableAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var result = await _context.AppointmentRequests
                                            .FromSqlRaw(@"
                                                SELECT * FROM appointment_requests 
                                                WHERE from_user = @userId AND table_id = @tableId 
                                                AND start_time = @start_time AND end_time = @end_time
                                                AND status NOT IN ('cancelled', 'rejected', 'expired') AND appointment_id IS NULL",
                                                new NpgsqlParameter("@userId", userId),
                                                new NpgsqlParameter("@tableId", tableId),
                                                new NpgsqlParameter("@start_time", DateTime.SpecifyKind(startTime, DateTimeKind.Unspecified)),
                                                new NpgsqlParameter("@end_time", DateTime.SpecifyKind(endTime, DateTimeKind.Unspecified)))
                                            .AsNoTracking()
                                            .Include(ar => ar.ToUserNavigation)
                                            .Include(ar => ar.Table)
                                            .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> GetAppointmentRequestsByTablesAppointmentIdAsync(int tablesAppointmentId)
        {
            try
            {
                var ta = await _context.TablesAppointments.AsNoTracking().SingleOrDefaultAsync(ta => ta.Id == tablesAppointmentId);

                return await _context.AppointmentRequests
                                    .Where(ar => ar.TableId == ta.TableId && ar.AppointmentId == ar.AppointmentId)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> GetAppointmentRequestsByAppointmentIdAsync(int appointmentId)
        {
            try
            {
                var taIds = _context.TablesAppointments
                                    .AsNoTracking()
                                    .Where(ta => ta.AppointmentId == appointmentId)
                                    .Select(ta => ta.TableId)
                                    .ToHashSet();

                return await _context.AppointmentRequests
                                    .AsNoTracking()
                                    .Where(ar => ar.AppointmentId == appointmentId && taIds.Contains(ar.TableId))
                                    .Include(ar => ar.ToUserNavigation)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(int userId, int tableAppointmentId)
        {
            try
            {
                var tablesAppointment = await _context.TablesAppointments.FindAsync(tableAppointmentId)
                                    ?? throw new Exception("Tables appointment with this ID does not exist.");

                var result = await _context.AppointmentRequests
                                            .Where(ar => ar.FromUser == userId && ar.TableId == tablesAppointment.TableId && ar.AppointmentId == tablesAppointment.AppointmentId)
                                            .Include(ar => ar.ToUserNavigation).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 

        public async Task<int> UpdateExpiredAppointmentRequests()
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    @"
                        UPDATE appointment_requests ar
                        SET status = 'expired'
                        WHERE expire_at <= @now AND (
                            ar.status = 'pending'
                            OR (
                                ar.status = 'accepted'
                                AND EXISTS (
                                    SELECT 1
                                    FROM tables_appointments ta
                                    JOIN payments p ON p.tables_appointment_id = ta.id
                                    WHERE ta.table_id = ar.table_id
                                      AND ta.appointment_id = ar.appointment_id
                                      AND p.status = 'unpaid'
                                )
                            )
                        );
                    ",
                    new NpgsqlParameter("@now", DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified))
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> CancelAllAppointmentRequestsFromUserOnTableAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var updatedRequests = await _context.AppointmentRequests
                    .FromSqlRaw(
                        "UPDATE appointment_requests " +
                        "SET status = 'cancelled' " +
                        "WHERE from_user = @userId AND table_id = @tableId " +
                        "AND start_time = @start_time AND end_time = @end_time AND status != 'expired' AND appointment_id IS NULL " +
                        "RETURNING *;",
                        new NpgsqlParameter("@userId", userId),
                        new NpgsqlParameter("@tableId", tableId),
                        new NpgsqlParameter("@start_time", DateTime.SpecifyKind(startTime, DateTimeKind.Unspecified)),
                        new NpgsqlParameter("@end_time", DateTime.SpecifyKind(endTime, DateTimeKind.Unspecified)))
                    .ToListAsync();

                return updatedRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
