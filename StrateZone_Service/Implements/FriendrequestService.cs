using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
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

        public async Task<FriendrequestModel> CreateFriendrequestAsync(FriendrequestRequest request)
        {
            try
            {
                FriendrequestModel model = new()
                { 
                    FromUser = request.FromUser,
                    ToUser = request.ToUser,
                    Status = PostgreEnums.RequestStatus.pending,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc)
                };

                var friendrequest = _mapper.Map<Friendrequest>(model);
                var result = await _friendRequestRepository.CreateFriendrequestAsync(friendrequest);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<FriendrequestModel> DeleteFriendrequestAsync(int id)
        {
            try
            {
                var result = await _friendRequestRepository.DeleteFriendrequestAsync(id);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<FriendrequestModel> GetFriendrequestByIdAsync(int id)
        {
            try
            {
                var result = await _friendRequestRepository.GetFriendrequestByIdAsync(id);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<PagedList<FriendrequestModel>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = await _friendRequestRepository.GetFriendrequestsOfUserIdAsync(parameters, id);
                var friendrequests = _mapper.Map<PagedList<FriendrequestModel>>(result);

                return new PagedList<FriendrequestModel>(friendrequests, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch
            {
                throw;
            }
        }

        public async Task<FriendrequestModel> UpdateFriendrequestAsync(FriendrequestModel friendrequestModel, int id)
        {
            try
            {
                var request = _mapper.Map<Friendrequest>(friendrequestModel);
                var result = await _friendRequestRepository.UpdateFriendrequestAsync(request, id);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }
    }
}
