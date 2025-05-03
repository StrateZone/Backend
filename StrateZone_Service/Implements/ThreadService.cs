using AutoMapper;
using StrateZone_Repository.Pagination;
using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System.Globalization;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Thread = StrateZone_Repository.Entities.Thread;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly ISystemService _systemService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ThreadService(IThreadRepository threadRepository, IImageService imageService, IMapper mapper, INotificationService notificationService, IThreadsTagService threadsTagService, ITagService tagsService, IUserService userService, ISystemService systemService, IServiceScopeFactory serviceScopeFactory)
        {
            _threadRepository = threadRepository;
            _imageService = imageService;
            _mapper = mapper;
            _notificationService = notificationService;
            _threadsTagService = threadsTagService;
            _tagsService = tagsService;
            _userService = userService;
            _systemService = systemService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<ThreadModel> CreateThreadAsync(ThreadRequest request)
        {
            try
            {
                var userRoleStr = (await _userService.GetUserByIdAsync((int)request.CreatedBy) ?? throw new Exception("This user does not exist"))
                                .UserRole;

                UserRole userRole = (UserRole)Enum.Parse(typeof(UserRole), userRoleStr);

                var tags = await _tagsService.GetTagsByIdsAsync(request.TagIds.ToArray());
                var bannedTag = tags.FirstOrDefault(t => (UserRole)Enum.Parse(typeof(UserRole), t.AllowedRole) > userRole);

                if (bannedTag != null)
                {
                    throw new Exception($"Bạn không được phép gắn thẻ \"{bannedTag.TagName}\" vào trong bài đăng của mình.");
                }

                ThreadStatus threadStatus = userRole >= UserRole.Staff ? ThreadStatus.published : ThreadStatus.pending;

                ThreadModel model = new()
                {
                    CreatedBy = request.CreatedBy,
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    Status = request.isDrafted ? ThreadStatus.drafted.ToString() : threadStatus.ToString(),
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

                foreach (var m in mapped)
                {
                    var counts = await _threadRepository.GetThreadLikeAndCommentCount(m.ThreadId);

                    m.LikesCount = counts.Item1;
                    m.IsUserLiked = counts.Item2;
                    m.CommentsCount = counts.Item3;
                }

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

                foreach (var m in mapped)
                {
                    var counts = await _threadRepository.GetThreadLikeAndCommentCount(m.ThreadId);

                    m.LikesCount = counts.Item1;
                    m.IsUserLiked = counts.Item2;
                    m.CommentsCount = counts.Item3;
                }

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

                foreach (var m in mapped)
                {
                    var counts = await _threadRepository.GetThreadLikeAndCommentCount(m.ThreadId);

                    m.LikesCount = counts.Item1;
                    m.IsUserLiked = counts.Item2;
                    m.CommentsCount = counts.Item3;
                }

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

                foreach (var m in mapped)
                {
                    var counts = await _threadRepository.GetThreadLikeAndCommentCount(m.ThreadId);

                    m.LikesCount = counts.Item1;
                    m.IsUserLiked = counts.Item2;
                    m.CommentsCount = counts.Item3;
                }

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

                foreach (var m in mapped)
                {
                    var counts = await _threadRepository.GetThreadLikeAndCommentCount(m.ThreadId);

                    m.LikesCount = counts.Item1;
                    m.IsUserLiked = counts.Item2;
                    m.CommentsCount = counts.Item3;
                }

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
                var mapped = _mapper.Map<ThreadModel>(result);

                mapped.LikesCount = mapped.Likes.Count;
                mapped.CommentsCount = mapped.Comments.Count;

                return mapped;
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

                var prevStatus = toApprove.Status;

                if (toApprove.Status != PostgreEnums.ThreadStatus.pending.ToString()
                    && toApprove.Status != PostgreEnums.ThreadStatus.edit_pending.ToString())
                    throw new Exception($"This thread is already {toApprove.Status}");

                toApprove.Status = PostgreEnums.ThreadStatus.published.ToString();
                var result = await UpdateThreadAsync(toApprove, toApprove.ThreadId);

                if (prevStatus == ThreadStatus.pending.ToString())
                {
                    var threadPoster = await _userService.GetUserByIdAsync((int)toApprove.CreatedBy);
                    var contrP = await _systemService.GetContributionPointsPerThread(1);
                    threadPoster.ContributionPoints += contrP;

                    await _userService.UpdateUserAsync(_mapper.Map<UserModel>(threadPoster), threadPoster.UserId);
                    
                    _ = Task.Run(async () => 
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        NotificationRequest notification = new()
                        {
                            ToUser = (int)result.CreatedBy,
                            Title = "Bài viết của bạn đã được phê duyệt!",
                            Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã được quản trị viên phê duyệt. Bạn được cộng {contrP} điểm đóng góp, " +
                            $"điểm này dùng để thể hiện độ.",
                            Type = PostgreEnums.NotificationType.thread,
                        };
                        await service.CreateNotificationAsync(notification);
                    });

                }
                else if (prevStatus == ThreadStatus.edit_pending.ToString())
                {
                    _ = Task.Run(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        NotificationRequest notification = new()
                        {
                            ToUser = (int)result.CreatedBy,
                            Title = "Bài viết của bạn đã được phê duyệt!",
                            Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã được quản trị viên phê duyệt.",
                            Type = PostgreEnums.NotificationType.thread,
                        };
                        await service.CreateNotificationAsync(notification);
                    });
                }

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

                if (toReject.Status != PostgreEnums.ThreadStatus.pending.ToString()
                    && toReject.Status != PostgreEnums.ThreadStatus.edit_pending.ToString())
                    throw new Exception($"This thread is already {toReject.Status}");

                if (toReject.Status == ThreadStatus.edit_pending.ToString() && toReject.UpdateOfThread != null)
                {
                    var previousVersion = await GetThreadByIdAsync((int)toReject.UpdateOfThread);
                    previousVersion.Status = ThreadStatus.published.ToString();
                    await UpdateThreadAsync(previousVersion, previousVersion.ThreadId);

                    toReject.Status = PostgreEnums.ThreadStatus.deleted.ToString();
                }
                else
                    toReject.Status = PostgreEnums.ThreadStatus.rejected.ToString();

                var result = await UpdateThreadAsync(toReject, toReject.ThreadId);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    NotificationRequest notification = new()
                    {
                        ToUser = (int)result.CreatedBy,
                        Title = "Bài viết của bạn đã bị từ chối!",
                        Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã bị từ chối. " +
                                $"Lưu ý: bài viết cần tuân thủ nghiêm ngặt các quy tắc của cộng đồng.",
                        Type = PostgreEnums.NotificationType.thread,
                    };

                    await service.CreateNotificationAsync(notification);
                });

                return _mapper.Map<ThreadModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> HideThreadAsync(int id)
        {
            try
            {
                var toReject = await _threadRepository.GetThreadByIdForAdminDeleteAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");

                var thread = _mapper.Map<ThreadModel>(toReject);
                if (thread.Status != PostgreEnums.ThreadStatus.published.ToString())
                    throw new Exception($"Can only hide published threads, this thread is {thread.Status}");

                thread.Status = PostgreEnums.ThreadStatus.hidden.ToString();
                var result = await UpdateThreadAsync(thread, toReject.ThreadId);

                NotificationRequest notification = new()
                {
                    ToUser = (int)result.CreatedBy,
                    Title = "Ẩn bài viết thành công!",
                    Content = $"Bạn đã ẩn bài viết có chủ đề \"{result.Title}\". " +
                    $"Để hiện lại bài viết, vui lòng truy cập vào mục \"Bài viết của tôi\".",
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

        public async Task<ThreadModel> EditThreadAsync(ThreadEditRequest threadModel, int id)
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

                var userRoleStr = (await _userService.GetUserByIdAsync((int)toBeUpdated.CreatedBy) ?? throw new Exception("This user does not exist")).UserRole;

                UserRole userRole = (UserRole)Enum.Parse(typeof(UserRole), userRoleStr);

                var tags = await _tagsService.GetTagsByIdsAsync(threadModel.TagIds.ToArray());
                var bannedTag = tags.FirstOrDefault(t => (UserRole)Enum.Parse(typeof(UserRole), t.AllowedRole) > userRole);

                if (bannedTag != null)
                {
                    throw new Exception($"Bạn không được phép gắn thẻ \"{bannedTag.TagName}\" vào trong bài đăng của mình.");
                }

                if (userRole < UserRole.Staff && (toBeUpdated.Status == ThreadStatus.published.ToString() || toBeUpdated.Status == ThreadStatus.hidden.ToString()))
                {
                    toBeUpdated.Status = ThreadStatus.deleted.ToString();
                    await UpdateThreadAsync(toBeUpdated, toBeUpdated.ThreadId);

                    ThreadModel newRequest = new ThreadModel()
                    { 
                        CreatedBy = toBeUpdated.CreatedBy,
                        Title = threadModel.Title,
                        Content = threadModel.Content,
                        Status = ThreadStatus.edit_pending.ToString(),
                        CreatedAt = toBeUpdated.CreatedAt,
                        ThumbnailUrl = toBeUpdated.ThumbnailUrl,
                        UpdateOfThread = toBeUpdated.ThreadId,
                        Comments = toBeUpdated.Comments,
                        Likes = toBeUpdated.Likes,
                        ThreadsTags = null,
                    };

                    var newThread = await _threadRepository.CreateThreadAsync(_mapper.Map<Thread>(newRequest));

                    var threadsTags = await _threadsTagService.CreateThreadsTagsAsync(threadModel.TagIds, newThread.ThreadId);

                    return _mapper.Map<ThreadModel>(newThread);
                }
                else
                {
                    toBeUpdated.Title = threadModel.Title;
                    toBeUpdated.Content = threadModel.Content;

                    var result = await _threadRepository.UpdateThreadAsync(_mapper.Map<Thread>(toBeUpdated), id);

                    await _threadsTagService.UpdateThreadsTagsAsync(threadModel.TagIds, toBeUpdated.ThreadId);

                    return _mapper.Map<ThreadModel>(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadModel> ShowThreadAsync(int id)
        {
            try
            {
                var toShow = await _threadRepository.GetThreadByIdForAdminDeleteAsync(id)
                                            ?? throw new Exception("Thread with this ID does not exist");

                var thread = _mapper.Map<ThreadModel>(toShow);
                if (thread.Status != PostgreEnums.ThreadStatus.hidden.ToString())
                    throw new Exception($"Can only show hidden threads, this thread is {thread.Status}");

                thread.Status = PostgreEnums.ThreadStatus.published.ToString();
                var result = await UpdateThreadAsync(thread, toShow.ThreadId);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadMonthResponse> GetAllThreadsWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                var allThreads = await _threadRepository.GetThreadsWithinMonthAsync(month, year);
                List<ThreadDailyResponse> threadDailyResponses = new();

                int dayInMonth = DateTime.DaysInMonth(year, month);
                for (int i = 1; i <= dayInMonth; ++i)
                {
                    int created = allThreads.Where(ta => ta.CreatedAt.Value.Day == i).Count();

                    int pending = allThreads.Where(ta => (ta.Status == ThreadStatus.pending || ta.Status == ThreadStatus.edit_pending) && ta.CreatedAt.Value.Day == i).Count();
                    int published = allThreads.Where(ta => ta.Status == ThreadStatus.published && ta.CreatedAt.Value.Day == i).Count();
                    int rejected = allThreads.Where(ta => ta.Status == ThreadStatus.rejected && ta.CreatedAt.Value.Day == i).Count();
                    int deleted = allThreads.Where(ta => ta.Status == ThreadStatus.deleted && ta.CreatedAt.Value.Day == i).Count();
                    int hidden = allThreads.Where(ta => ta.Status == ThreadStatus.hidden && ta.CreatedAt.Value.Day == i).Count();

                    threadDailyResponses.Add(new()
                    {
                        DayOfMonth = i,
                        ThreadsCreated = created,
                        PendingCount = pending,
                        RejectedCount = rejected,
                        DeletedCount = deleted,
                        HiddenCount = hidden,
                        PublishedCount = published,
                    });
                }

                return new()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalDays = dayInMonth,
                    ThreadDailyResponse = threadDailyResponses
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
