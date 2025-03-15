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
    public class RoomRepository : IRoomRepository
    {
        private readonly StrateZoneDbContext _context;

        public RoomRepository( StrateZoneDbContext context )
        {
            _context = context;
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            try
            {
                using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using var cmd = new NpgsqlCommand(@"
                            INSERT INTO rooms (room_name, room_type, description, capacity, status) 
                            VALUES (@name, @type::room_type, @description, @capacity, @status::room_status)
                            RETURNING room_id;", connection);

                    cmd.Parameters.AddWithValue("@name", room.RoomName);
                    cmd.Parameters.AddWithValue("@type", room.Type.ToString());
                    cmd.Parameters.AddWithValue("@description", room.Description);
                    cmd.Parameters.AddWithValue("@capacity", room.Capacity);
                    cmd.Parameters.AddWithValue("@status", room.Status.ToString());

                    var newRoomId = await cmd.ExecuteScalarAsync();
                    room.RoomId = Convert.ToInt32(newRoomId);
                }

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
                var rooms = _context.Rooms.Include(r => r.Tables).AsQueryable();
                return await PagedList<Room>.ToPagedList(rooms, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Room>> GetRoomsByTypeAsync(RoomParameters parameters, PostgreEnums.RoomType roomType)
        {
            try
            {
                var rooms = _context.Rooms
                                    .FromSqlRaw(
                                            @"SELECT * FROM rooms WHERE room_type = @room_type::room_type",
                                        new NpgsqlParameter("@room_type", roomType.ToString())
                                    )
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

                using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using var cmd = new NpgsqlCommand(@"
                            UPDATE rooms SET 
                                room_name = @name, 
                                room_type = @type::room_type, 
                                description = @description, 
                                capacity = @capacity, 
                                status = @status::room_status 
                            WHERE room_id = @id
                            RETURNING room_id;", connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", room.RoomName);
                    cmd.Parameters.AddWithValue("@type", room.Type.ToString());
                    cmd.Parameters.AddWithValue("@description", room.Description);
                    cmd.Parameters.AddWithValue("@capacity", room.Capacity);
                    cmd.Parameters.AddWithValue("@status", room.Status.ToString());

                    await cmd.ExecuteScalarAsync();

                    var updatedRoom = await _context.Rooms.FindAsync(id);
                    return updatedRoom;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
