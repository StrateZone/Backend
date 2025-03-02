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
    public class GameExtensionRepository : IGameExtensionRepository
    {
        private readonly StrateZoneDbContext _context;

        public GameExtensionRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameExtension>> GetGameExtensionsAsync()
        {
            try
            {
                return await _context.GameExtensions.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameExtension> GetGameExtensionByIdAsync(int id)
        {
            try
            {
                return await _context.GameExtensions.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GameExtension>> GetGameExtensionsByGameTypeIdAsync(int id)
        {
            try
            {
                return await _context.GameExtensions
                                     .Where(ge => ge.TypeId == id)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
