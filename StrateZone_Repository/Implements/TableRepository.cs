using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class TableRepository : ITableRepository
    {
        private readonly StrateZoneDbContext _context;

        public TableRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Table>> GetTablesAsync(TableParameters parameters)
        {
            try
            {
                var tables = _context.Tables
                                    .Include(t => t.GameType)
                                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tables, parameters.PageNumber, parameters.PageSize);
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
                                    .Include(t => t.Room)
                                    .FirstOrDefaultAsync() ?? throw new Exception("No table with this ID was found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Table>> GetTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType)
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

                var tables = _context.Tables
                                    .Where(t => t.GameTypeId == expectedId)
                                    .Include(t => t.GameType)
                                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tables, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Table>> GetAvailableTablesAsync(TableParameters parameters)
        {
            try
            {
                var query = @"
                            SELECT t.*
                            FROM tables t
                            JOIN rooms r ON t.room_id = r.room_id
                            WHERE r.status = 'available'
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                JOIN appointments a ON ta.appointment_id = a.appointment_id
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND a.status NOT IN ('cancelled', 'completed', 'expired')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tables, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedList<Table>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var query = @"
                            SELECT t.*
                            FROM tables t
                            JOIN rooms r ON t.room_id = r.room_id
                            JOIN ""gameTypes"" gt ON gt.type_name = @TypeName::game_type
                            WHERE r.status = 'available' AND gt.type_id = t.""gameType_id""
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                JOIN appointments a ON ta.appointment_id = a.appointment_id
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND a.status NOT IN ('cancelled', 'completed', 'expired')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@TypeName", gameType.ToString()),
                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tables, parameters.PageNumber, parameters.PageSize);
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
                if (await _context.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.TableId == id) == null) throw new Exception("Table with this ID does not exist");

                table.TableId = id;

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

        public async Task<PagedList<Table>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(
             TableParameters parameters,
             GameTypeEnum[] gameTypes = null,
             RoomType[] roomTypes = null)
        {
            try
            {
                var query = @"
                    SELECT t.*
                    FROM tables t
                    JOIN rooms r ON t.room_id = r.room_id
                    JOIN ""gameTypes"" gt ON gt.type_id = t.""gameType_id""
                    WHERE r.status = 'available' 
                    AND (@GameTypeIds IS NULL OR gt.type_name = ANY(@GameTypeIds::public.game_type[]))
                    AND (@RoomTypeIds IS NULL OR r.room_type = ANY(@RoomTypeIds::public.room_type[]))
                    AND NOT EXISTS (
                        SELECT 1
                        FROM tables_appointments ta
                        JOIN appointments a ON ta.appointment_id = a.appointment_id
                        WHERE ta.table_id = t.table_id
                        AND ta.schedule_time < @EndTime
                        AND ta.end_time > @StartTime
                        AND a.status NOT IN ('cancelled', 'completed', 'expired')
                    )";

                var gameTypeNames = gameTypes?.Select(gt => gt.ToString()).ToArray();
                var roomTypeNames = roomTypes?.Select(rt => rt.ToString()).ToArray();

                var tablesQuery = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@GameTypeIds", NpgsqlDbType.Array | NpgsqlDbType.Text)
                        { Value = gameTypeNames ?? (object)DBNull.Value },

                        new NpgsqlParameter("@RoomTypeIds", NpgsqlDbType.Array | NpgsqlDbType.Text)
                        { Value = roomTypeNames ?? (object)DBNull.Value },

                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .AsNoTracking()
                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tablesQuery, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<GameTypeEnum, List<Table>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount)
        {
            try
            {
                var query = @"
                            WITH RankedTables AS (
                                SELECT t.*, 
                                        ROW_NUMBER() OVER (PARTITION BY t.""gameType_id"" ORDER BY t.table_id) AS row_num
                                FROM tables t
                                JOIN rooms r ON t.room_id = r.room_id AND r.room_type != 'study'
                                WHERE r.status = 'available'
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM tables_appointments ta
                                    JOIN appointments a ON ta.appointment_id = a.appointment_id
                                    WHERE ta.table_id = t.table_id
                                    AND ta.schedule_time < @EndTime
                                    AND ta.end_time > @StartTime
                                    AND a.status NOT IN ('cancelled', 'completed', 'expired')
                                )
                            )
                            SELECT * FROM RankedTables WHERE row_num <= @TableCount";

                var tables = await _context.Tables
                        .FromSqlRaw(query,
                            new NpgsqlParameter("@StartTime", parameters.StartTime),
                            new NpgsqlParameter("@EndTime", parameters.EndTime),
                            new NpgsqlParameter("@TableCount", tableCount))
                        .Include(t => t.GameType)
                        .Include(t => t.Room)
                        .AsNoTracking()
                        .ToListAsync();

                var groupedTables = tables
                    .GroupBy(t => t.GameType.TypeName)
                    .ToDictionary(g => g.Key, g => g.ToList());

                return groupedTables;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching available tables: {ex.Message}", ex);
            }
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            return await _context.Tables.ToListAsync();
        }

        public Task<List<Table>> GetAvailableTablesAsync(DateTime StartTime, DateTime EndTime)
        {
            try
            {
                var query = @"
                            SELECT t.*
                            FROM tables t
                            JOIN rooms r ON t.room_id = r.room_id
                            WHERE r.status = 'available'
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                JOIN appointments a ON ta.appointment_id = a.appointment_id
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND a.status NOT IN ('cancelled', 'completed', 'expired')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@StartTime", StartTime),
                        new NpgsqlParameter("@EndTime", EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .ToListAsync();

                return tables;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
