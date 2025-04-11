using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IFriendlistService
    {
        Task<FriendlistModel> AddFriendAsync(FriendlistModel friend);
        Task<FriendlistModel> DeleteFriendAsync(int id);
        Task<FriendlistModel> GetFriendByIdAsync(int id);
        Task<PagedList<FriendlistResponse>> GetFriendsByUserIdAsync(TablesAppointmentParameters parameters, int userId);
        Task<FriendlistModel> UpdateFriendAsync(FriendlistModel friendlist, int id);
    }
}
