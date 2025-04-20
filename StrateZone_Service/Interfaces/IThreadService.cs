using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Interfaces
{
    public interface IThreadService
    {
        Task<ThreadModel> CreateThreadAsync(ThreadRequest thread);
        Task<ThreadModel> DeleteThreadAsync(int id);
        Task<PagedList<ThreadModel>> GetAllThreadsAsync(TablesAppointmentParameters parameters);
        Task<PagedList<ThreadModel>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, int id);
        Task<PagedList<ThreadModel>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, ThreadStatus[] statuses, int id);
        Task<PagedList<ThreadModel>> GetAllThreadsByStatusesAsync(TablesAppointmentParameters parameters, ThreadStatus[] statuses);
        Task<PagedList<ThreadModel>> GetAllThreadsByStatusesAndTagsAsync(ThreadParameters parameters);
        Task<ThreadModel> GetThreadByIdAsync(int id);
        Task<ThreadModel> UpdateThreadAsync(ThreadModel thread, int id);
        Task<ThreadModel> ApproveThreadAsync(int id);
        Task<ThreadModel> RejectThreadAsync(int id);
        Task<ThreadModel> AdminHideThreadAsync(int id);
    }
}
