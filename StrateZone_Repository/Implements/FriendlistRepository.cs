using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
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
    public class FriendlistRepository : IFriendlistRepository
    {
        private readonly StrateZoneDbContext _context;

        public FriendlistRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Friendlist> GetFriendByIdAsync(int id)
        {
            try
            {
                return await _context.Friendlists
                                    .AsNoTracking()
                                    .Include(f => f.User)
                                    .Include(f => f.Friend)
                                    .SingleOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<Friendlist>> GetFriendsByUserIdAsync(TablesAppointmentParameters parameters, int userId)
        {
            try
            {
                var result =  _context.Friendlists
                                    .AsNoTracking()
                                    .Where(f => f.UserId == userId || f.FriendId == userId)
                                    .Include(f => f.User)
                                    .Include(f => f.Friend)
                                    .AsQueryable();

                return await PagedList<Friendlist>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Friendlist> AddFriendAsync(Friendlist friend)
        {
            try
            {
                await _context.Friendlists.AddAsync(friend);
                await _context.SaveChangesAsync();

                return friend;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Friendlist> UpdateFriendAsync(Friendlist friendlist, int id)
        {
            try
            {
                if (!(_context.Friendlists.AsNoTracking().Any(f => f.Id == id)))
                    throw new Exception("Friendlist with this ID does not exists");

                friendlist.Id = id;
                _context.Friendlists.Update(friendlist);
                await _context.SaveChangesAsync();
                return friendlist;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Friendlist> DeleteFriendAsync(int id)
        {
            try
            {
                var friend = await _context.Friendlists.FindAsync(id)
                    ?? throw new Exception("Friendlist with this ID does not exists");

                _context.Friendlists.Remove(friend);
                await _context.SaveChangesAsync();

                return friend;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
