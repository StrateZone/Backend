using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface IThreadRepository
    {
        Task<Entities.Thread> CreateThreadAsync(Entities.Thread thread);
        Task<Entities.Thread> DeleteThreadAsync(int id);
        Task<PagedList<Entities.Thread>> GetAllThreadsAsync(TablesAppointmentParameters parameters);
        Task<PagedList<Entities.Thread>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, int id);
        Task<PagedList<Entities.Thread>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, ThreadStatus[] statuses, int id);
        Task<PagedList<Entities.Thread>> GetAllThreadsByStatusesAsync(TablesAppointmentParameters parameters, ThreadStatus[] statuses);
        Task<PagedList<Entities.Thread>> GetAllThreadsByStatusesAndTagsAsync(ThreadParameters parameters);
        Task<Entities.Thread> GetThreadByIdAsync(int id);
        Task<Entities.Thread> GetThreadByIdForAdminDeleteAsync(int id);
        Task<Entities.Thread> UpdateThreadAsync(Entities.Thread thread, int id);
        Task<List<Entities.Thread>> GetThreadsWithinMonthAsync(int month, int year);
        Task<(int, bool, int)> GetThreadLikeAndCommentCount(int id);
    }
}