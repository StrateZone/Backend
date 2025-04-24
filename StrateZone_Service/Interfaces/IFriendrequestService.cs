using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IFriendrequestService
    {
        Task<PagedList<FriendrequestModel>> GetFriendrequestsFromUserIdAsync(FriendrequestParameters parameters, int id);
        Task<PagedList<FriendrequestModel>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id);
        Task<FriendrequestModel> GetFriendrequestByIdAsync(int id);
        Task<FriendrequestModel> GetFriendrequestBySenderAndReceiverIdAsync(int senderId, int receiverId);
        Task<FriendrequestModel> CreateFriendrequestAsync(FriendrequestRequest friendrequestModel);
        Task<FriendrequestModel> UpdateFriendrequestAsync(FriendrequestModel friendrequestModel, int id);
        Task<FriendrequestModel> AcceptFriendrequestAsync(int id);
        Task<FriendrequestModel> RejectFriendrequestAsync(int id);
        Task<FriendrequestModel> DeleteFriendrequestAsync(int id);
        Task<FriendrequestModel> DeleteFriendrequestByUserAndFriendIdAsync(int senderId, int receiverId);
    }
}
