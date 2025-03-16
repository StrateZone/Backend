using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class FriendrequestService : IFriendrequestService
    {
        private readonly IFriendrequestRepository _friendRequestRepository;
        private readonly IMapper _mapper;

        public FriendrequestService(IFriendrequestRepository friendRequestRepository, IMapper mapper)
        {
            _friendRequestRepository = friendRequestRepository;
            _mapper = mapper;
        }

        public Task<FriendrequestModel> CreateFriendrequestAsync(FriendrequestModel friendrequestModel)
        {
            throw new NotImplementedException();
        }

        public Task<FriendrequestModel> DeleteFriendrequestAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<FriendrequestModel> GetFriendrequestByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FriendrequestModel>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id)
        {
            throw new NotImplementedException();
        }

        public Task<FriendrequestModel> UpdateFriendrequestAsync(FriendrequestModel friendrequestModel, int id)
        {
            throw new NotImplementedException();
        }
    }
}
