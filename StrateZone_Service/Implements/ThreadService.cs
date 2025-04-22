using AutoMapper;
using MealHunt_Repositories.Pagination;
using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Thread = StrateZone_Repository.Entities.Thread;

namespace StrateZone_Service.Implements
{
    public class ThreadService : IThreadService
    {
        private readonly IThreadRepository _threadRepository;
        private readonly INotificationService _notificationService;
        private readonly IImageService _imageService;
        private readonly IThreadsTagService _threadsTagService;
        private readonly IMapper _mapper;
        private readonly ITagService _tagsService;
        private readonly IUserService _userService;
        
        public ThreadService(IThreadRepository threadRepository, IImageService imageService, IMapper mapper, INotificationService notificationService, IThreadsTagService threadsTagService, ITagService tagsService, IUserService userService)
        {
            _threadRepository = threadRepository;
            _imageService = imageService;
            _mapper = mapper;
            _notificationService = notificationService;
            _threadsTagService = threadsTagService;
            _tagsService = tagsService;
            _userService = userService;
        }

        public async Task<ThreadModel> CreateThreadAsync(ThreadRequest request)
        {
            try
            {
                var userRoleStr = (await _userService.GetUserByIdAsync((int)request.CreatedBy) ?? throw new Exception("This user does not exist"))
                                .UserRole;

                UserRole userRole = (UserRole) Enum.Parse(typeof(UserRole), userRoleStr);

                var tags = await _tagsService.GetTagsByIdsAsync(request.TagIds.ToArray());
                var bannedTag = tags.FirstOrDefault(t => (UserRole)Enum.Parse(typeof(UserRole), t.AllowedRole) > userRole);

                if (bannedTag != null)
                {
                    throw new Exception($"Bạn không được phép gắn thẻ \"{bannedTag.TagName}\" vào trong bài đăng của mình.");
                }

                ThreadModel model = new()
                {
                    CreatedBy = request.CreatedBy,
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    Status = request.isDrafted ? ThreadStatus.drafted.ToString() : ThreadStatus.pending.ToString(),
                    Rating = 0,
                };

                var thread = _mapper.Map<Thread>(model);
                var result = await _threadRepository.CreateThreadAsync(thread);

                var threadsTags = await _threadsTagService.CreateThreadsTagsAsync(request.TagIds, result.ThreadId);
                model.ThreadsTags = threadsTags;

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> DeleteThreadAsync(int id)
        {
            try 
            {
                var result = await _threadRepository.DeleteThreadAsync(id);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<ThreadModel>> GetAllThreadsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var threads = await _threadRepository.GetAllThreadsAsync(parameters);
                var mapped = _mapper.Map<PagedList<ThreadModel>>(threads);

                return new PagedList<ThreadModel>(mapped, threads.TotalCount, threads.CurrentPage, threads.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<ThreadModel>> GetAllThreadsByStatusesAndTagsAsync(ThreadParameters parameters)
        {
            try
            {
                var threads = await _threadRepository.GetAllThreadsByStatusesAndTagsAsync(parameters);
                var mapped = _mapper.Map<PagedList<ThreadModel>>(threads);

                return new PagedList<ThreadModel>(mapped, threads.TotalCount, threads.CurrentPage, threads.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<ThreadModel>> GetAllThreadsByStatusesAsync(TablesAppointmentParameters parameters, PostgreEnums.ThreadStatus[] statuses)
        {
            try
            {
                var threads = await _threadRepository.GetAllThreadsByStatusesAsync(parameters, statuses);
                var mapped = _mapper.Map<PagedList<ThreadModel>>(threads);

                return new PagedList<ThreadModel>(mapped, threads.TotalCount, threads.CurrentPage, threads.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<ThreadModel>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, int id)
        {
            try
            {
                var threads = await _threadRepository.GetThreadsByUserIdAsync(parameters, id);
                var mapped = _mapper.Map<PagedList<ThreadModel>>(threads);

                return new PagedList<ThreadModel>(mapped, threads.TotalCount, threads.CurrentPage, threads.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<ThreadModel>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, ThreadStatus[] statuses, int id)
        {
            try
            {
                var threads = await _threadRepository.GetThreadsByUserIdAsync(parameters, statuses, id);
                var mapped = _mapper.Map<PagedList<ThreadModel>>(threads);

                return new PagedList<ThreadModel>(mapped, threads.TotalCount, threads.CurrentPage, threads.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> GetThreadByIdAsync(int id)
        {
            try
            {
                var result = await _threadRepository.GetThreadByIdAsync(id);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> ApproveThreadAsync(int id)
        {
            try
            {
                var toApprove = await GetThreadByIdAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");

                if (toApprove.Status != PostgreEnums.ThreadStatus.pending.ToString())
                    throw new Exception($"This thread is already {toApprove.Status}");

                toApprove.Status = PostgreEnums.ThreadStatus.published.ToString();
                var result = await UpdateThreadAsync(toApprove, toApprove.ThreadId);

                var threadPoster = await _userService.GetUserByIdAsync((int)toApprove.CreatedBy);
                threadPoster.Points += 10;
                threadPoster.ContributionPoints += 50;
                await _userService.UpdateUserAsync(_mapper.Map<UserModel>(threadPoster), threadPoster.UserId);

                NotificationRequest notification = new()
                {
                    ToUser = (int)result.CreatedBy,
                    Title = "Bài viết của bạn đã được phê duyệt!",
                    Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã được quản trị viên phê duyệt.  Bạn được cộng 25 điểm cá nhân, " +
                    $"điểm khi tích đủ có thể dùng để đổi sang vouchers giảm giá cho lần đặt hẹn kế tiếp.",
                    Type = PostgreEnums.NotificationType.thread,
                };
                await _notificationService.CreateNotificationAsync(notification);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> RejectThreadAsync(int id)
        {
            try
            {
                var toReject = await GetThreadByIdAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");

                if (toReject.Status != PostgreEnums.ThreadStatus.pending.ToString())
                    throw new Exception($"This thread is already {toReject.Status}");

                toReject.Status = PostgreEnums.ThreadStatus.rejected.ToString();
                var result = await UpdateThreadAsync(toReject, toReject.ThreadId);

                NotificationRequest notification = new()
                {
                    ToUser = (int)result.CreatedBy,
                    Title = "Bài viết của bạn đã bị từ chối!",
                    Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã bị từ chối. " +
                    $"Lưu ý: bài viết cần tuân thủ nghiêm ngặt các quy tắc của cộng đồng.",
                    Type = PostgreEnums.NotificationType.thread,
                };
                await _notificationService.CreateNotificationAsync(notification);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> AdminHideThreadAsync(int id)
        {
            try
            {
                var toReject = await _threadRepository.GetThreadByIdForAdminDeleteAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");
                var thread = _mapper.Map<ThreadModel>(toReject);
                if (thread.Status != PostgreEnums.ThreadStatus.published.ToString())
                    throw new Exception($"This thread is already {thread.Status}");

                thread.Status = PostgreEnums.ThreadStatus.hidden.ToString();
                var result = await UpdateThreadAsync(thread, toReject.ThreadId);

                NotificationRequest notification = new()
                {
                    ToUser = (int)result.CreatedBy,
                    Title = "Bài viết của bạn đã bị ẩn!",
                    Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã bị ẩn bởi admin. " +
                    $"Lưu ý: bài viết cần tuân thủ nghiêm ngặt các quy tắc của cộng đồng.",
                    Type = PostgreEnums.NotificationType.thread,
                };
                await _notificationService.CreateNotificationAsync(notification);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> UpdateThreadAsync(ThreadModel threadModel, int id)
        {
            try
            {
                var thread = _mapper.Map<Thread>(threadModel);
                var result = await _threadRepository.UpdateThreadAsync(thread, id);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> EditThreadAsync(ThreadModel threadModel, int id)
        {
            try
            {
                var toBeUpdated = await GetThreadByIdAsync(id)
                                ?? throw new Exception("Thread with this ID does not exist");

                if (toBeUpdated.Status != ThreadStatus.published.ToString()
                    && toBeUpdated.Status != ThreadStatus.pending.ToString()
                    && toBeUpdated.Status != ThreadStatus.edit_pending.ToString()
                    && toBeUpdated.Status != ThreadStatus.drafted.ToString())
                    throw new Exception($"This thread is currently {toBeUpdated.Status}");

                if (toBeUpdated.CreatedBy != threadModel.CreatedBy)
                    throw new Exception("Creator before and after update doesn't match.");

                threadModel.Status = ThreadStatus.edit_pending.ToString();
                var thread = _mapper.Map<Thread>(threadModel);
                var result = await _threadRepository.UpdateThreadAsync(thread, id);

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
