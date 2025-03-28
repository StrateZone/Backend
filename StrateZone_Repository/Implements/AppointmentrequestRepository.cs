using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Reflection.Metadata;
using System.Text;

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
                                    .Where(ar => ar.ToUser == userId)
                                    .Include(ar => ar.ToUserNavigation)
                                    .AsQueryable();
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
                                    .Where(ar => ar.FromUser == userId)
                                    .Include(ar => ar.FromUserNavigation)
                                    .Include(ar => ar.ToUserNavigation)
                                    .AsQueryable();
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
                                                    && ar.ToUser == appointmentRequest.ToUser 
                                                    && ar.TableId == appointmentRequest.TableId
                                                    && (ar.AppointmentId == appointmentRequest.AppointmentId ||
                                                    (ar.AppointmentId == null && appointmentRequest.AppointmentId == null))
                                                )
                                                .ToListAsync();

                if (requestsList.Any(r => r.Status == PostgreEnums.RequestStatus.pending))
                    throw new Exception($"Appointment invitation to this user already been sent.");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var createCmd = connection.CreateCommand();

                createCmd.CommandText = @"
                    INSERT INTO appointment_requests (from_user, to_user, table_id, appointment_id, status, start_time, end_time, expire_at, created_at) 
                    VALUES (@from_user, @to_user, @table_id, @appointment_id, @status::request_status, @start_time, @end_time, @expire_at, @created_at)
                    RETURNING id;"
                ;

                createCmd.Parameters.Add(new NpgsqlParameter("@from_user", appointmentRequest.FromUser));
                createCmd.Parameters.Add(new NpgsqlParameter("@to_user", appointmentRequest.ToUser));
                createCmd.Parameters.Add(new NpgsqlParameter("@table_id", appointmentRequest.TableId));
                createCmd.Parameters.Add(new NpgsqlParameter("@appointment_id", appointmentRequest.AppointmentId == null ? DBNull.Value : appointmentRequest.AppointmentId));
                createCmd.Parameters.Add(new NpgsqlParameter("@status", appointmentRequest.Status.ToString()));
                createCmd.Parameters.Add(new NpgsqlParameter("@start_time", appointmentRequest.StartTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@end_time", appointmentRequest.EndTime));
                createCmd.Parameters.Add(new NpgsqlParameter("@expire_at", appointmentRequest.ExpireAt));
                createCmd.Parameters.Add(new NpgsqlParameter("@created_at", appointmentRequest.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newAppointmentId = await createCmd.ExecuteScalarAsync();
                appointmentRequest.Id = Convert.ToInt32(newAppointmentId);

                return appointmentRequest;
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

                var updatedAppointmentRequest = await _context.AppointmentRequests.FindAsync(id);
                return updatedAppointmentRequest;
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
                                "AND table_id = @table_id AND (appointment_id IS NULL OR appointment_id = @appointment_id) THEN 'rejected' " +
                            "ELSE status " +
                    "END;");
                parameters.Add(new NpgsqlParameter("@id", id));
                parameters.Add(new NpgsqlParameter("@user_id", toAccept.FromUser));
                parameters.Add(new NpgsqlParameter("@table_id", toAccept.TableId));
                parameters.Add(new NpgsqlParameter("@appointment_id", toAccept.AppointmentId != null ? toAccept.AppointmentId : DBNull.Value));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(toAccept).State = EntityState.Detached;

                return await _context.AppointmentRequests.FindAsync(id);
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

                return await _context.AppointmentRequests.FindAsync(id);
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
                var result = _context.AppointmentRequests.Where(ar => ar.TableId == tableId).AsQueryable();
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
                var result = _context.AppointmentRequests.Where(ar => ar.AppointmentId == appointmentId && ar.TableId == tableId).AsQueryable();
                return await PagedList<Appointmentrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(int userId, int tableId)
        {
            try
            {
                var result = await _context.AppointmentRequests
                                            .FromSqlRaw(@"
                                                SELECT * FROM appointment_requests 
                                                WHERE from_user = {0} AND table_id = {1} AND status NOT IN ('cancelled', 'rejected', 'expired') AND appointment_id IS NULL",
                                                userId,
                                                tableId)
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
                    "UPDATE appointment_requests ar SET status = 'expired' WHERE ar.status = 'pending' AND ar.expire_at <= {0};",
                    DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointmentrequest>> CancelAllAppointmentRequestsFromUserOnTableAsync(int userId, int tableId)
        {
            try
            {
                var updatedRequests = await _context.AppointmentRequests
                    .FromSqlRaw(
                        "UPDATE appointment_requests " +
                        "SET status = 'cancelled' " +
                        "WHERE from_user = {0} AND table_id = {1} AND status != 'expired' AND appointment_id IS NULL " +
                        "RETURNING *;", 
                        userId,
                        tableId)
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
