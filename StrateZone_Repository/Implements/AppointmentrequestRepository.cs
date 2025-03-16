using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var result = _context.AppointmentRequests.Where(ar => ar.ToUser == userId).AsQueryable();
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
                var result = _context.AppointmentRequests.Where(ar => ar.FromUser == userId).AsQueryable();
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
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO appointment_requests (from_user, to_user, appointment_id, status, created_at) 
                    VALUES (@from_user, @to_user, @appointment_id, @status::request_status, @created_at)
                    RETURNING id;"
                ;

                cmd.Parameters.Add(new NpgsqlParameter("@from_user", appointmentRequest.FromUser));
                cmd.Parameters.Add(new NpgsqlParameter("@to_user", appointmentRequest.ToUser));
                cmd.Parameters.Add(new NpgsqlParameter("@appointment_id", appointmentRequest.AppointmentId));
                cmd.Parameters.Add(new NpgsqlParameter("@status", appointmentRequest.Status.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@created_at", appointmentRequest.CreatedAt ?? DateTime.UtcNow));

                var newAppointmentId = await cmd.ExecuteScalarAsync();
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
                var existingPrice = await _context.Prices.FindAsync(id) ?? throw new Exception("Appointment request with this ID does not exist");

                _context.Entry(existingPrice).State = EntityState.Detached;

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

                if (appointmentRequest.AppointmentId > 0)
                {
                    sql.Append("appointment_id = @appointment_id, ");
                    parameters.Add(new NpgsqlParameter("@appointment_id", appointmentRequest.AppointmentId));
                }

                sql.Append("status = @status::request_status, ");
                parameters.Add(new NpgsqlParameter("@status", appointmentRequest.Status.ToString()));

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
    }
}
