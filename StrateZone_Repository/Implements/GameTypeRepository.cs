using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class GameTypeRepository : IGameTypeRepository
    {
        private readonly StrateZoneDbContext _context;

        public GameTypeRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameType>> GetGameTypesAsync()
        {
            try
            {
                return await _context.GameTypes.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameType> GetGameTypesByIdAsync(int id)
        {
            try
            {
                return await _context.GameTypes.AsNoTracking().FirstOrDefaultAsync(g => g.TypeId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GameType>> GetGameTypesWithExtensionsAsync()
        {
            try
            {
                return await _context.GameTypes.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameType> GetGameTypeWithExtensionsByIdAsync(int id)
        {
            try
            {
                return await _context.GameTypes.AsNoTracking().Where(gt => gt.TypeId == id)
                                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
