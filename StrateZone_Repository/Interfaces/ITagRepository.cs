using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> DeleteTagAsync(int id);
        Task<Tag> UpdateTagAsync(Tag tag, int tagId);
        Task<Tag> GetTagByIdAsync(int id);
        Task<List<Tag>> GetTagsAsync();
        Task<List<Tag>> GetThreadTagsAsync();
        Task<List<Tag>> GetProductTagsAsync();
        Task<List<Tag>> SearchTagsAsync(string content);
    }
}