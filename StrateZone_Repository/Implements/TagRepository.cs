using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class TagRepository : ITagRepository
    {
        private readonly StrateZoneDbContext _context;

        public TagRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            try
            {
                return await _context.Tags.AsNoTracking()
                                        .Where(tag => tag.Status == Parameters.PostgreEnums.TagStatus.active)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Tag>> GetThreadTagsAsync()
        {
            try
            {
                return await _context.Tags.AsNoTracking().Where(tag => tag.ThreadsTags.Any()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Tag>> GetProductTagsAsync()
        {
            try
            {
                return await _context.Tags.AsNoTracking().Where(tag => tag.ProductTags.Any()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            try
            {
                return await _context.Tags.AsNoTracking().SingleOrDefaultAsync(t => t.TagId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(int[] ids)
        {
            try
            {
                return await _context.Tags.AsNoTracking()
                                        .Where(t => ids.Contains(t.TagId))
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Tag>> GetTagsByUserRoleAsync(PostgreEnums.UserRole role)
        {
            try
            {
                var allTags = await _context.Tags.AsNoTracking().ToListAsync();

                return allTags
                    .Where(t => role >= t.AllowedRole)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Tag>> SearchTagsAsync(string content)
        {
            try
            {
                content = content.ToLower();
                return await _context.Tags.AsNoTracking()
                                .Where(t => t.TagName.ToLower()
                                .Contains(content))
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            try
            {
                await _context.Tags.AddAsync(tag);
                await _context.SaveChangesAsync();

                return tag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Tag> DeleteTagAsync(int id)
        {
            try
            {
                var toDelete = await _context.Tags.FindAsync(id)
                            ?? throw new Exception("Tag with this ID does not exist");

                _context.Tags.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Tag> UpdateTagAsync(Tag tag, int tagId)
        {
            try
            {
                if ((await _context.Tags.AsNoTracking().SingleOrDefaultAsync(t => t.TagId == tagId)) == null)
                    throw new Exception("Thread with this ID does not eixst");

                tag.TagId = tagId;

                _context.Tags.Update(tag);

                await _context.SaveChangesAsync();
                return tag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
