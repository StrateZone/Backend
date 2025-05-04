using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class PointsHistoryRepository : IPointsHistoryRepository
    {
        private readonly StrateZoneDbContext _context;

        public PointsHistoryRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PointsHistory> AddAsync(PointsHistory history)
        {
            await _context.PointsHistories.AddAsync(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<PagedList<PointsHistory>> GetAllAsync(TablesAppointmentParameters parameters)
        {
            var result = _context.PointsHistories
                .AsNoTracking()
                .Include(p => p.OfUserNavigation)
                .AsQueryable();

            return await PagedList<PointsHistory>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PointsHistory> GetByIdAsync(int id)
        {
            return await _context.PointsHistories
                .AsNoTracking()
                .Include(p => p.OfUserNavigation)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new Exception("No point history with this ID was found.");
        }

        public async Task<PagedList<PointsHistory>> GetByUserIdAsync(int userId, TablesAppointmentParameters parameters)
        {
            var result = _context.PointsHistories
                .AsNoTracking()
                .Where(p => p.OfUser == userId)
                .AsQueryable();

            return await PagedList<PointsHistory>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
        }

        public async Task UpdateAsync(PointsHistory history, int id)
        {
            if (!await _context.PointsHistories.AsNoTracking().AnyAsync(p => p.Id == id))
                throw new Exception("No point history with this ID was found.");

            _context.PointsHistories.Update(history);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var history = await _context.PointsHistories.FindAsync(id)
                ?? throw new Exception("No point history with this ID was found.");

            _context.PointsHistories.Remove(history);
            await _context.SaveChangesAsync();
        }
    }
}
