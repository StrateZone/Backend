using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface IRoomService
    {
        Task<RoomResponse> CreateRoomAsync(RoomRequest request);
        Task<RoomResponse> DeleteRoomAsync(int id);
        Task<RoomResponse> GetRoomByIdAsync(int id);
        Task<PagedList<RoomResponse>> GetRoomsAsync(RoomParameters parameters);
        Task<PagedList<RoomResponse>> GetRoomsByRoomTypeAsync(RoomParameters parameters, PostgreEnums.RoomType roomType);
        Task<RoomResponse> UpdateRoomAsync(RoomModel roomModel, int id);
    }
}