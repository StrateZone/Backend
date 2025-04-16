using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface ITagService
    {
        Task<TagModel> CreateTagAsync(TagModel tag);
        Task<TagModel> DeleteTagAsync(int id);
        Task<TagModel> UpdateTagAsync(TagModel tagModel, int id);
        Task<TagModel> AdminActivateTagAsync(int id);
        Task<TagModel> AdminHideTagAsync(int id);
        Task<TagModel> GetTagByIdAsync(int id);
        Task<List<TagModel>> GetTagsAsync();
        Task<List<TagModel>> SearchTagsAsync(string content);
        Task<List<TagModel>> GetThreadTagsAsync();
        Task<List<TagModel>> GetProductTagsAsync();
    }
}
