using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IThreadsTagService
    {
        Task<ThreadsTagModel> CreateThreadsTagAsync(ThreadsTagModel threadsTag);
        Task<List<ThreadsTagModel>> CreateThreadsTagsAsync(List<ThreadsTagModel> threadsTags);
        Task<List<ThreadsTagModel>> CreateThreadsTagsAsync(HashSet<int> TagIds, int threadId);
        Task<List<ThreadsTagModel>> UpdateThreadsTagsAsync(HashSet<int> TagIds, int threadId);
        Task<ThreadsTagModel> DeleteThreadsTagAsync(int id);
        Task<ThreadsTagModel> UpdateThreadsTagAsync(ThreadsTagModel threadsTag, int id);
        Task<List<ThreadsTagModel>> UpdateThreadsTagsAsync(List<ThreadsTagModel> threadsTags, int threadId);
    }
}
