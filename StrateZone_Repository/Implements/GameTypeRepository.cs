using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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

        public async Task<GameType> AddAsync(GameType gameType)
        {
            try
            {
                if (await _context.GameTypes.AsNoTracking().AnyAsync(g => g.TypeName == gameType.TypeName))
                    throw new Exception("Gametype with this name already exists");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO ""gameTypes"" (type_name) 
                VALUES (@type_name)
                RETURNING type_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@type_name", gameType.TypeName));

                var newAppointmentId = await cmd.ExecuteScalarAsync();
                int typeId = Convert.ToInt32(newAppointmentId);

                gameType.TypeId = typeId;

                return gameType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameType> DeleteAsync(int id)
        {
            try
            {
                var gameType = await _context.GameTypes.FindAsync(id) ?? throw new Exception("Gametype with this ID does not exist");
                
                _context.Remove(gameType);
                await _context.SaveChangesAsync();

                return gameType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
