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
    public class FriendrequestRepository : IFriendrequestRepository
    {
        private readonly StrateZoneDbContext _context;

        public FriendrequestRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Friendrequest> CreateFriendrequestAsync(Friendrequest friendrequest)
        {
            try
            {
                if (_context.Friendlists.AsNoTracking().Any(fl => (fl.UserId == friendrequest.FromUser && fl.FriendId == friendrequest.ToUser) 
                                                    || (fl.UserId == friendrequest.FromUser && fl.FriendId == friendrequest.ToUser)))
                    throw new Exception($"You two are already friend with each other.");

                var requestsList = await _context.Friendrequests.AsNoTracking().FirstOrDefaultAsync(ar => ar.FromUser == friendrequest.FromUser && ar.ToUser == friendrequest.ToUser);
                if (requestsList != null && requestsList.Status == PostgreEnums.RequestStatus.pending)
                    throw new Exception($"Friend request to this user already been sent.");

                using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
                {

                    if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                    using var cmd = new NpgsqlCommand(@"
                            INSERT INTO friendrequests (from_user, to_user, status, created_at) 
                            VALUES (@from_user, @to_user, @status::request_status, @createdAt)
                            RETURNING id;", connection);

                    cmd.Parameters.AddWithValue("@from_user", friendrequest.FromUser);
                    cmd.Parameters.AddWithValue("@to_user", friendrequest.ToUser);
                    cmd.Parameters.AddWithValue("@status", friendrequest.Status.ToString());
                    cmd.Parameters.AddWithValue("@createdAt", friendrequest.CreatedAt ?? DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));

                    var newUserId = await cmd.ExecuteScalarAsync();
                    friendrequest.Id = Convert.ToInt32(newUserId);
                }

                return friendrequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Friendrequest> DeleteFriendrequestAsync(int id)
        {
            try
            {
                var toDelete = await _context.Friendrequests.FindAsync(id) ?? throw new Exception("Friendrequest with this ID doesn't exist.");
                
                _context.Friendrequests.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Friendrequest> GetFriendrequestByIdAsync(int id)
        {
            try
            {
                return await _context.Friendrequests
                                    .Where(x => x.Id == id)
                                    .Include(fr => fr.FromUserNavigation)
                                    .Include(fr => fr.ToUserNavigation)
                                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Friendrequest>> GetFriendrequestsFromUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = _context.Friendrequests
                                    .Where(fr => fr.FromUser == id)
                                    .Include(fr => fr.FromUserNavigation)
                                    .Include(fr => fr.ToUserNavigation)
                                    .AsQueryable();

                return await PagedList<Friendrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Friendrequest>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = _context.Friendrequests
                                    .Where(fr => fr.ToUser == id)
                                    .Include(fr => fr.FromUserNavigation)
                                    .Include(fr => fr.ToUserNavigation)
                                    .AsQueryable();

                return await PagedList<Friendrequest>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Friendrequest> UpdateFriendrequestAsync(Friendrequest friendrequest, int id)
        {
            try
            {
                var existingFriendrequest = await _context.Friendrequests.FindAsync(id) ?? throw new Exception("Friend request with this ID does not exist");

                friendrequest.Id = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE friendrequests SET ");

                if (friendrequest.FromUser > 0)
                {
                    sql.Append("from_user = @from_user, ");
                    parameters.Add(new NpgsqlParameter("@from_user", friendrequest.FromUser));
                }

                if (friendrequest.ToUser > 0)
                {
                    sql.Append("to_user = @to_user, ");
                    parameters.Add(new NpgsqlParameter("@to_user", friendrequest.ToUser));
                }

                sql.Append("status = @status::request_status, ");
                parameters.Add(new NpgsqlParameter("@status", friendrequest.Status.ToString()));

                if (friendrequest.CreatedAt.HasValue)
                {
                    sql.Append("created_at = @created_at, ");
                    parameters.Add(new NpgsqlParameter("@created_at", friendrequest.CreatedAt.Value));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var updatedFriendRequest = await _context.Friendrequests.FindAsync(id);
                return updatedFriendRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
