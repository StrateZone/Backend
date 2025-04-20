using StrateZone_Repository.Entities;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> DeleteTagAsync(int id);
        Task<Tag> UpdateTagAsync(Tag tag, int tagId);
        Task<Tag> GetTagByIdAsync(int id);
        Task<List<Tag>> GetTagsByIdsAsync(int[] isd);
        Task<List<Tag>> GetTagsAsync();
        Task<List<Tag>> GetTagsByUserRoleAsync(UserRole role);
        Task<List<Tag>> GetThreadTagsAsync();
        Task<List<Tag>> GetProductTagsAsync();
        Task<List<Tag>> SearchTagsAsync(string content);
    }
}