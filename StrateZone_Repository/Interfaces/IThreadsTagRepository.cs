using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IThreadsTagRepository
    {
        Task<ThreadsTag> CreateThreadsTagAsync(ThreadsTag threadsTag);
        Task<List<ThreadsTag>> CreateThreadsTagsAsync(List<ThreadsTag> threadsTags);
        Task<ThreadsTag> DeleteThreadsTagAsync(int id);
        Task<ThreadsTag> UpdateThreadsTagAsync(ThreadsTag threadsTag, int id);
    }
}