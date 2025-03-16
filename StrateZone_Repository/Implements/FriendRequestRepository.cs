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
                using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using var cmd = new NpgsqlCommand(@"
                            INSERT INTO friendrequests (from_user, to_user, status, created_at) 
                            VALUES (@from_user, @to_user, @status::request_status, @createdAt)
                            RETURNING id;", connection);

                    cmd.Parameters.AddWithValue("@from_user", friendrequest.FromUser);
                    cmd.Parameters.AddWithValue("@to_user", friendrequest.ToUser);
                    cmd.Parameters.AddWithValue("@status", friendrequest.Status.ToString());
                    cmd.Parameters.AddWithValue("@createdAt", friendrequest.CreatedAt ?? DateTime.UtcNow);

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

        public Task<Friendrequest> UpdateFriendrequestAsync(Friendrequest friendrequest, int id)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
