using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;
using System.Buffers;

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
                                    .AsNoTracking()
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
                return await _context.Tables.AsNoTracking()
                                    .Include(t => t.GameType)
                                    .Include(t => t.Room)
                                    .FirstOrDefaultAsync(t => t.TableId == id) ?? throw new Exception("No table with this ID was found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Table> GetSimilarTableByIdAsync(DateTime startTime, DateTime endTime, int id)
        {
            try
            {
                var availableTables = await GetAvailableTablesAsync(startTime, endTime);

                // Get the original table to find its GameType and Room
                var originalTable = await _context.Tables
                    .Include(t => t.Room)
                    .Include(t => t.GameType)
                    .FirstOrDefaultAsync(t => t.TableId == id) 
                    ?? throw new Exception("Original table not found.");

                if (availableTables.Contains(originalTable)) return originalTable;

                var gameTypeId = originalTable.GameTypeId;
                var roomId = originalTable.RoomId;
                var roomType = originalTable.Room?.Type;

                // Try to find a table in the same room and same game type
                var sameRoomTable = availableTables
                    .Where(t => t.TableId != id && t.RoomId == roomId && t.GameTypeId == gameTypeId)
                    .FirstOrDefault();

                if (sameRoomTable != null)
                    return sameRoomTable;

                // If not found, try to find table in rooms with the same room type and same game type
                var sameRoomTypeTable = availableTables
                    .Where(t => t.TableId != id &&
                                t.GameTypeId == gameTypeId &&
                                t.Room != null &&
                                t.Room.Type == roomType)
                    .FirstOrDefault();

                if (sameRoomTypeTable != null)
                    return sameRoomTypeTable;

                throw new Exception("Không còn bàn tương tự hiện đang khả dụng. Vui lòng tìm bàn khác.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetSimilarTableByIdAsync: {ex.Message}", ex);
            }
        }


        public async Task<PagedList<Table>> GetTablesByGameTypeAsync(TableParameters parameters, string gameType)
        {
            try
            {
                var expectedId = await _context.GameTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(gt => gt.TypeName == gameType) ?? throw new Exception("Game type not found.");

                var tables = _context.Tables
                                    .Where(t => t.GameTypeId == expectedId.TypeId)
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
                            WHERE t.status = 'active' AND r.status = 'available'
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND ta.status NOT IN ('cancelled', 'completed', 'expired', 'refunded')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .OrderBy(t => t.Room.RoomName)
                    .AsNoTracking()
                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tables, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedList<Table>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, string gameType)
        {
            try
            {
                var query = @"
                            SELECT t.*
                            FROM tables t
                            JOIN rooms r ON t.room_id = r.room_id
                            JOIN ""gameTypes"" gt ON gt.type_name = @TypeName
                            WHERE t.status = 'active' AND r.status = 'available' 
                            AND (@RoomName IS NULL OR r.room_name LIKE CONCAT('%', @RoomName, '%')) 
                            AND gt.type_id = t.""gameType_id"" AND gt.status = 'active'
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND ta.status NOT IN ('cancelled', 'completed', 'expired', 'refunded')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@TypeName", gameType.ToString()),
                        new NpgsqlParameter("@RoomName", NpgsqlDbType.Text) { Value = string.IsNullOrEmpty(parameters.RoomName) ? DBNull.Value : parameters.RoomName },
                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .OrderBy(t => t.Room.RoomName)
                    .AsNoTracking()
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
             string[] gameTypes = null,
             string[] roomTypes = null)
        {
            try
            {
                var query = @"
                    SELECT t.*
                    FROM tables t
                    JOIN rooms r ON t.room_id = r.room_id
                    JOIN ""gameTypes"" gt ON gt.type_id = t.""gameType_id""
                    WHERE t.status = 'active' AND r.status = 'available' 
                    AND (@RoomName IS NULL OR r.room_name LIKE CONCAT('%', @RoomName, '%'))
                    AND ((@GameTypeIds IS NULL OR gt.type_name = ANY(@GameTypeIds)) AND gt.status = 'active')
                    AND (@RoomTypeIds IS NULL OR r.room_type = ANY(@RoomTypeIds))
                    AND NOT EXISTS (
                        SELECT 1
                        FROM tables_appointments ta
                        WHERE ta.table_id = t.table_id
                        AND ta.schedule_time < @EndTime
                        AND ta.end_time > @StartTime
                        AND ta.status NOT IN ('cancelled', 'completed', 'expired', 'refunded')
                    )";

                var gameTypeNames = gameTypes?.Select(gt => gt.ToString()).ToArray();
                var roomTypeNames = roomTypes?.Select(rt => rt.ToString()).ToArray();

                var tablesQuery = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@GameTypeIds", NpgsqlDbType.Array | NpgsqlDbType.Text)
                        { Value = gameTypeNames ?? (object)DBNull.Value },

                        new NpgsqlParameter("@RoomTypeIds", NpgsqlDbType.Array | NpgsqlDbType.Text)
                        { Value = roomTypeNames ?? (object)DBNull.Value },

                        new NpgsqlParameter("@RoomName", NpgsqlDbType.Text)
                        { Value = string.IsNullOrEmpty(parameters.RoomName) ? DBNull.Value : parameters.RoomName },

                        new NpgsqlParameter("@StartTime", parameters.StartTime),
                        new NpgsqlParameter("@EndTime", parameters.EndTime))
                    .AsNoTracking()
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .OrderBy(t => t.Room.RoomName)
                    .AsQueryable();

                return await PagedList<Table>.ToPagedList(tablesQuery, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<string, List<Table>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount)
        {
            try
            {
                var query = @"
                            WITH RankedTables AS (
                                SELECT t.*, 
                                        ROW_NUMBER() OVER (PARTITION BY t.""gameType_id"" ORDER BY t.table_id) AS row_num
                                FROM tables t
                                JOIN rooms r ON t.room_id = r.room_id AND r.room_type != 'study'
                                WHERE t.status = 'active' AND t.status = 'active'
                                AND r.status = 'available' AND (@RoomName IS NULL OR r.room_name LIKE CONCAT('%', @RoomName, '%'))
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM tables_appointments ta
                                    WHERE ta.table_id = t.table_id
                                    AND ta.schedule_time < @EndTime
                                    AND ta.end_time > @StartTime
                                    AND ta.status NOT IN ('cancelled', 'completed', 'expired', 'refunded')
                                )
                            )
                            SELECT * FROM RankedTables WHERE row_num <= @TableCount";

                var tables = await _context.Tables
                        .FromSqlRaw(query,
                            new NpgsqlParameter("@StartTime", parameters.StartTime),
                            new NpgsqlParameter("@EndTime", parameters.EndTime),
                            new NpgsqlParameter("@RoomName", NpgsqlDbType.Text)
                                { Value = string.IsNullOrEmpty(parameters.RoomName) ? DBNull.Value : parameters.RoomName },
                            new NpgsqlParameter("@TableCount", tableCount))
                        .Include(t => t.GameType)
                        .Include(t => t.Room)
                        .OrderBy(t => t.Room.RoomName)
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

        public async Task<PagedList<Table>> GetTablesAsync(TablesAppointmentParameters parameters, string? search)
        {

            var result = _context.Tables.AsNoTracking()
            .Where(t => search == null || t.TableId.ToString().Equals(search))
                                    .AsQueryable();
            return await PagedList<Table>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
        }

        public Task<List<Table>> GetAvailableTablesAsync(DateTime StartTime, DateTime EndTime)
        {
            try
            {
                var query = @"
                            SELECT t.*
                            FROM tables t
                            JOIN rooms r ON t.room_id = r.room_id
                            WHERE t.status = 'active' AND r.status = 'available'
                            AND NOT EXISTS (
                                SELECT 1
                                FROM tables_appointments ta
                                WHERE ta.table_id = t.table_id
                                AND ta.schedule_time < @EndTime
                                AND ta.end_time > @StartTime
                                AND ta.status NOT IN ('cancelled', 'completed', 'expired', 'refunded')
                            )";

                var tables = _context.Tables
                    .FromSqlRaw(query,
                        new NpgsqlParameter("@StartTime", StartTime),
                        new NpgsqlParameter("@EndTime", EndTime))
                    .Include(t => t.GameType)
                    .Include(t => t.Room)
                    .OrderBy(t => t.Room.RoomName)
                    .ToListAsync();

                return tables;
            }
            catch
            {
                throw;
            }
        }

        public async Task DisableTablesOnRoomAsync(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE tables SET status='out_of_service' WHERE room_id = {0};",
                        id
                    );
            }
            catch
            {
                throw;
            }
        }

        public async Task EnableTablesOnRoomAsync(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE tables t SET status='active' WHERE room_id = {0} 
                        AND EXISTS (
                            SELECT 1 FROM ""gameTypes"" gt WHERE gt.type_id = t.""gameType_id"" AND gt.status = 'active'
                        );",
                        id
                    );
            }
            catch
            {
                throw;
            }
        }

        public async Task DisableTablesOnGametypeAsync(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE tables SET status='out_of_service' WHERE ""gameType_id"" = {0};",
                        id
                    );
            }
            catch
            {
                throw;
            }
        }

        public async Task EnableTablesOnGametypeAsync(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE tables t SET status='active' WHERE t.""gameType_id"" = {0} AND EXISTS (SELECT 1 FROM rooms r WHERE r.room_id = t.room_id AND r.status='available');",
                        id
                    );
            }
            catch
            {
                throw;
            }
        }
    }
}
