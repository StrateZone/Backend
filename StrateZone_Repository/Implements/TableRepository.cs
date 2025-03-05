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
    public class TableRepository : ITableRepository
    {
        private readonly StrateZoneDbContext _context;

        public TableRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            try
            {
                return await _context.Tables
                                    .Include(t => t.GameType)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Table> GetTableByIdAsync(int id)
        {
            try
            {
                return await _context.Tables.Where(t => t.TableId == id)
                                    .Include(t => t.GameType)
                                    .FirstOrDefaultAsync() ?? throw new Exception("No table with this ID was found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Table>> GetTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                string query =
                    @"
                        SELECT g.type_id FROM public.""gameTypes"" AS g 
                        WHERE g.type_name = @p0::public.game_type
                        LIMIT 1
                    ";

                var expectedId = await _context.GameTypes
                    .FromSqlRaw(query, gameType.ToString())
                    .Select(g => g.TypeId)
                    .FirstOrDefaultAsync();

                if (expectedId == null) throw new Exception("Game type not found.");

                return await _context.Tables
                    .Where(t => t.GameTypeId == expectedId)
                    .Include(t => t.GameType)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Table>> GetAvailableTablesAsync()
        {
            try
            {
                var currentTime = DateTime.Now;

                return await _context.Tables
                                .Where(t => !t.TablesAppointments.Any() ||
                                            t.TablesAppointments.All(ta => ta.Appointment.EndTime < currentTime))
                                .Include(t => t.GameType)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Table>> GetAvailableTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var currentTime = DateTime.Now;
                string query =
                    @"
                        SELECT g.type_id FROM public.""gameTypes"" AS g 
                        WHERE g.type_name = @p0::public.game_type
                        LIMIT 1
                    ";

                var expectedId = await _context.GameTypes
                    .FromSqlRaw(query, gameType.ToString())
                    .Select(g => g.TypeId)
                    .FirstOrDefaultAsync();

                if (expectedId == null) throw new Exception("Game type not found.");

                return await _context.Tables
                                .Where(t => !t.TablesAppointments.Any() ||
                                            t.TablesAppointments.All(ta => ta.Appointment.EndTime < currentTime))
                                .Include(t => t.GameType)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Table> CreateTableAsync(Table table)
        {
            try
            {
                await _context.Tables.AddAsync(table);
                await _context.SaveChangesAsync();

                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Table> UpdateTableAsync(Table table, int id)
        {
            try
            {
                if (await _context.Tables.FindAsync(id) == null) throw new Exception("Table with this ID does not exist");

                _context.Tables.Update(table);
                await _context.SaveChangesAsync();

                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Table> DeleteTableAsync(int id)
        {
            try
            {
                var toDelete = await _context.Tables.FindAsync(id) ?? throw new Exception("Table with this ID does not exist");

                _context.Tables.Remove(toDelete);
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
