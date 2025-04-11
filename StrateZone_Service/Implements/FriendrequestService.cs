using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class FriendrequestService : IFriendrequestService
    {
        private readonly IFriendrequestRepository _friendRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IFriendlistService _friendlistService;
        private readonly IMapper _mapper;

        public FriendrequestService(IFriendrequestRepository friendRequestRepository, IMapper mapper, IFriendlistService friendlistService, INotificationService notificationService)
        {
            _friendRequestRepository = friendRequestRepository;
            _mapper = mapper;
            _friendlistService = friendlistService;
            _notificationService = notificationService;
        }

        public async Task<FriendrequestModel> CreateFriendrequestAsync(FriendrequestRequest request)
        {
            try
            {
                FriendrequestModel model = new()
                { 
                    FromUser = request.FromUser,
                    ToUser = request.ToUser,
                    Status = PostgreEnums.RequestStatus.pending.ToString(),
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                };

                var friendrequest = _mapper.Map<Friendrequest>(model);
                var result = await _friendRequestRepository.CreateFriendrequestAsync(friendrequest);

                NotificationRequest toUser_notification = new()
                {
                    ToUser = result.ToUser,
                    Title = $"Bạn có một lời mời kết bạn đến từ {result.FromUserNavigation.Username}!",
                    Content = $"{result.FromUserNavigation.Username} đã gửi cho bạn lời mời kết bạn. Bấm để xem chi tiết",
                    Type = NotificationType.friend_request,
                };
                await _notificationService.CreateNotificationAsync(toUser_notification);

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

        public async Task<PagedList<FriendrequestModel>> GetFriendrequestsFromUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = await _friendRequestRepository.GetFriendrequestsFromUserIdAsync(parameters, id);
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

        public async Task<FriendrequestModel> AcceptFriendrequestAsync(int id)
        {
            try
            {
                var requestModel = await GetFriendrequestByIdAsync(id);
                if (requestModel.Status != PostgreEnums.RequestStatus.pending.ToString())
                {
                    throw new Exception($"This request is already {requestModel.Status}");
                }

                requestModel.Status = RequestStatus.accepted.ToString();
                var request = _mapper.Map<Friendrequest>(requestModel);
                var result = await _friendRequestRepository.UpdateFriendrequestAsync(request, id);

                await _friendlistService.AddFriendAsync(new FriendlistModel()
                {
                    UserId = result.FromUser,
                    FriendId = result.ToUser,
                });

                NotificationRequest fromUser_notification = new()
                {
                    ToUser = requestModel.FromUser,
                    Title = "Lời mời kết bạn đã được chấp nhận",
                    Content = $"{requestModel.ToUserNavigation.Username} đã " +
                    $"chấp nhận lời mời kết bạn của bạn.",
                    Type = NotificationType.friend,
                };
                await _notificationService.CreateNotificationAsync(fromUser_notification);

                NotificationRequest toUser_notification = new()
                {
                    ToUser = requestModel.ToUser,
                    Title = $"Chấp nhận lời mời kết bạn thành công!",
                    Content = $"Bạn đã chấp nhận lời mời kết bạn đến từ {requestModel.FromUserNavigation.Username}. " +
                    $"Cả hai bây giờ đã là bạn bè.",
                    Type = NotificationType.friend,
                };
                await _notificationService.CreateNotificationAsync(toUser_notification);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<FriendrequestModel> RejectFriendrequestAsync(int id)
        {
            try
            {
                var requestModel = await GetFriendrequestByIdAsync(id);
                if (requestModel.Status != PostgreEnums.RequestStatus.pending.ToString())
                {
                    throw new Exception($"This request is already {requestModel.Status}");
                }

                requestModel.Status = RequestStatus.rejected.ToString();
                var request = _mapper.Map<Friendrequest>(requestModel);
                var result = await _friendRequestRepository.UpdateFriendrequestAsync(request, id);

                NotificationRequest toUser_notification = new()
                {
                    ToUser = result.ToUser,
                    Title = $"Từ chối lời mời kết bạn thành công!",
                    Content = $"Bạn đã từ chối lời mời kết bạn đến từ {requestModel.FromUserNavigation.Username}.",
                    Type = NotificationType.friend_request,
                };
                await _notificationService.CreateNotificationAsync(toUser_notification);

                return _mapper.Map<FriendrequestModel>(result);
            }
            catch
            {
                throw;
            }
        }
    }
}
