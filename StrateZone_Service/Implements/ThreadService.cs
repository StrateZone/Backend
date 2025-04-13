using AutoMapper;
using MealHunt_Repositories.Pagination;
using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
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

        public ThreadService(IThreadRepository threadRepository, IImageService imageService, IMapper mapper, INotificationService notificationService, IThreadsTagService threadsTagService)
        {
            _threadRepository = threadRepository;
            _imageService = imageService;
            _mapper = mapper;
            _notificationService = notificationService;
            _threadsTagService = threadsTagService;
        }

        public async Task<ThreadModel> CreateThreadAsync(ThreadRequest request)
        {
            try
            {
                ThreadModel model = new()
                {
                    CreatedBy = request.CreatedBy,
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    Status = PostgreEnums.ThreadStatus.pending.ToString(),
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

        public async Task<PagedList<ThreadModel>> GetAllThreadsByStatusesAndTagsAsync(TablesAppointmentParameters parameters, PostgreEnums.ThreadStatus[] statuses, HashSet<int> TagIds, int? userId)
        {
            try
            {
                var threads = await _threadRepository.GetAllThreadsByStatusesAndTagsAsync(parameters, statuses, TagIds, userId);
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
                var toReject = await GetThreadByIdAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");

                if (toReject.Status != PostgreEnums.ThreadStatus.pending.ToString())
                    throw new Exception($"This thread is already {toReject.Status}");

                toReject.Status = PostgreEnums.ThreadStatus.published.ToString();
                var result = await UpdateThreadAsync(toReject, toReject.ThreadId);

                NotificationRequest notification = new()
                {
                    ToUser = (int)result.CreatedBy,
                    Title = "Bài viết của bạn đã được phê duyệt!",
                    Content = $"Bài viết của bạn với chủ đề \"{result.Title}\" đã được quản trị viên phê duyệt.",
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

    }
}
