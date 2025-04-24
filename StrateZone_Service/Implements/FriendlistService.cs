using AutoMapper;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class FriendlistService : IFriendlistService
    {
        private readonly IFriendlistRepository _friendlistRepository;
        private readonly IMapper _mapper;
    
        public FriendlistService(IFriendlistRepository friendlistRepository, IMapper mapper)
        {
            _friendlistRepository = friendlistRepository;
            _mapper = mapper;
        }

        public async Task<FriendlistModel> AddFriendAsync(FriendlistModel friend)
        {
            try
            {
                var friendlist = _mapper.Map<Friendlist>(friend);
                var result = await _friendlistRepository.AddFriendAsync(friendlist);
            
                return _mapper.Map<FriendlistModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<FriendlistModel> DeleteFriendAsync(int id)
        {
            try
            {
                var result = await _friendlistRepository.DeleteFriendAsync(id);

                return _mapper.Map<FriendlistModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<FriendlistModel> GetFriendByIdAsync(int id)
        {
            try
            {
                var result = await _friendlistRepository.GetFriendByIdAsync(id);

                return _mapper.Map<FriendlistModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<FriendlistResponse>> GetFriendsByUserIdAsync(TablesAppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _friendlistRepository.GetFriendsByUserIdAsync(parameters, userId);

                var mapped = _mapper.Map<List<FriendlistModel>>(result);
            
                List<FriendlistResponse> responses = new List<FriendlistResponse>();
                foreach (var item in mapped)
                {
                    FriendlistResponse friendlist = new()
                    {
                        Id = item.Id,
                        FriendId = item.UserId == userId ? item.FriendId : item.UserId,
                        Friend = _mapper.Map<UserResponse>(item.UserId == userId ? item.Friend : item.User),
                    };

                    responses.Add(friendlist);
                }

                return new PagedList<FriendlistResponse>(responses, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async  Task<FriendlistModel> UpdateFriendAsync(FriendlistModel friendlist, int id)
        {
            try
            {
                var fl = _mapper.Map<Friendlist>(friendlist);
                var result = await _friendlistRepository.UpdateFriendAsync(fl, id);

                return _mapper.Map<FriendlistModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
