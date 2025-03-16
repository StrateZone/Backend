using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IFriendrequestService
    {
        Task<PagedList<FriendrequestModel>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id);
        Task<FriendrequestModel> GetFriendrequestByIdAsync(int id);
        Task<FriendrequestModel> CreateFriendrequestAsync(FriendrequestModel friendrequestModel);
        Task<FriendrequestModel> UpdateFriendrequestAsync(FriendrequestModel friendrequestModel, int id);
        Task<FriendrequestModel> DeleteFriendrequestAsync(int id);
    }
}
