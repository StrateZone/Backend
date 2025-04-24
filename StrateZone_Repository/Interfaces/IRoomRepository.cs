using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface IRoomRepository
    {
        Task<PagedList<Room>> GetRoomsAsync(RoomParameters parameters);
        Task<PagedList<Room>> GetRoomsByTypeAsync(RoomParameters parameters, RoomType roomType);
        Task<Room> GetRoomByIdAsync(int id);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room, int id);
        Task<Room> DeleteRoomAsync(int id);
    }
}
