using StrateZone_Repository.Pagination;
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
    public class RoomRepository : IRoomRepository
    {
        private readonly StrateZoneDbContext _context;

        public RoomRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            try
            {
                if (!await _context.Prices.AsNoTracking().AnyAsync(p => p.RoomType == room.Type))
                    throw new Exception("Room type does not exist, add a room type first.");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO rooms (room_name, room_type, description, capacity, status, is_for_monthly_booking) 
                    VALUES (@name, @type, @description, @capacity, @status::room_status, @is_for_monthly_booking)
                    RETURNING room_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@name", room.RoomName));
                cmd.Parameters.Add(new NpgsqlParameter("@type", room.Type.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@description", room.Description));
                cmd.Parameters.Add(new NpgsqlParameter("@capacity", room.Capacity));
                cmd.Parameters.Add(new NpgsqlParameter("@status", room.Status.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@is_for_monthly_booking", room.IsForMonthlyBooking));

                var newRoomId = await cmd.ExecuteScalarAsync();
                room.RoomId = Convert.ToInt32(newRoomId);

                return room;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Room> DeleteRoomAsync(int id)
        {
            try
            {
                var toDelete = await _context.Rooms.FindAsync(id) ?? throw new Exception("No room with this ID was found");

                _context.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            try
            {
                return await _context.Rooms
                                    .AsNoTracking()
                                    .Include(r => r.Tables)
                                    .FirstOrDefaultAsync(r => r.RoomId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Room>> GetRoomsAsync(RoomParameters parameters)
        {
            try
            {
                var rooms = _context.Rooms.AsNoTracking().Include(r => r.Tables).AsQueryable();
                return await PagedList<Room>.ToPagedList(rooms, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Room>> GetRoomsByTypeAsync(RoomParameters parameters, string roomType)
        {
            try
            {
                var rooms = _context.Rooms
                                    .FromSqlRaw(
                                            @"SELECT * FROM rooms WHERE room_type = @room_type",
                                        new NpgsqlParameter("@room_type", roomType.ToString())
                                    )
                                    .AsNoTracking()
                                    .Include(r => r.Tables)
                                    .AsQueryable();

                return await PagedList<Room>.ToPagedList(rooms, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Room> UpdateRoomAsync(Room room, int id)
        {
            try
            {
                var existingRoom = await _context.Rooms.FindAsync(id) ?? throw new Exception("Room with this ID does not exist");

                _context.Entry(existingRoom).State = EntityState.Detached;

                room.RoomId = id;

                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE rooms SET ");

                if (!string.IsNullOrEmpty(room.RoomName))
                {
                    sql.Append("room_name = @room_name, ");
                    parameters.Add(new NpgsqlParameter("@room_name", room.RoomName));
                }

                if (room.Capacity.HasValue)
                {
                    sql.Append("capacity = @capacity, ");
                    parameters.Add(new NpgsqlParameter("@capacity", room.Capacity));
                }

                if (!string.IsNullOrEmpty(room.Description))
                {
                    sql.Append("description = @description, ");
                    parameters.Add(new NpgsqlParameter("@description", room.Description));
                }

                sql.Append("room_type = @room_type, ");
                parameters.Add(new NpgsqlParameter("@room_type", room.Type.ToString()));

                sql.Append("status = @status::room_status, ");
                parameters.Add(new NpgsqlParameter("@status", room.Status.ToString()));

                sql.Append("is_for_monthly_booking = @is_for_monthly_booking, ");
                parameters.Add(new NpgsqlParameter("@is_for_monthly_booking", room.IsForMonthlyBooking));

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE room_id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var updatedRoom = await _context.Rooms.FindAsync(id);
                return updatedRoom;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> GetAllRoomtypesAsync()
        {
            return await _context.Prices.AsNoTracking()
                            .Where(r => r.RoomType != null)
                            .GroupBy(r => r.RoomType)
                            .Select(r => r.Key)
                            .ToListAsync();
        }

        public async Task<List<string>> GetRegularRoomtypesAsync()
        {
            var regularRoomTypes = await _context.Rooms
                .AsNoTracking()
                .Where(r => !r.IsForMonthlyBooking && r.Tables.Count > 0 && r.Status == PostgreEnums.RoomStatus.available)
                .Select(r => r.Type)
                .Distinct()
                .ToListAsync();

            return await _context.Prices
                .AsNoTracking()
                .Where(p => regularRoomTypes.Contains(p.RoomType))
                .GroupBy(r => r.RoomType)
                .Select(r => r.Key)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetMonthlyRoomtypes()
        {
            var regularRoomTypes = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.IsForMonthlyBooking && r.Tables.Count > 0 && r.Status == PostgreEnums.RoomStatus.available)
                .Select(r => r.Type)
                .Distinct()
                .ToListAsync();

            return await _context.Prices
                .AsNoTracking()
                .Where(p => regularRoomTypes.Contains(p.RoomType))
                .GroupBy(r => r.RoomType)
                .Select(r => r.Key)
                .Distinct()
                .ToListAsync();
        }
    }
}
