using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class ThreadsTagRepository : IThreadsTagRepository
    {
        private readonly StrateZoneDbContext _context;

        public ThreadsTagRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<ThreadsTag> CreateThreadsTagAsync(ThreadsTag threadsTag)
        {
            try
            {
                await _context.ThreadsTags.AddAsync(threadsTag);
                await _context.SaveChangesAsync();

                return threadsTag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ThreadsTag>> CreateThreadsTagsAsync(List<ThreadsTag> threadsTags)
        {
            try
            {
                await _context.ThreadsTags.AddRangeAsync(threadsTags);
                await _context.SaveChangesAsync();

                return threadsTags;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadsTag> DeleteThreadsTagAsync(int id)
        {
            try
            {
                var toDelete = await _context.ThreadsTags.FindAsync(id)
                    ?? throw new Exception("Threadstag with this ID does not exist");

                _context.ThreadsTags.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadsTag> UpdateThreadsTagAsync(ThreadsTag threadsTag, int id)
        {
            try
            {
                if (!await _context.ThreadsTags.AsNoTracking().AnyAsync(tt => tt.Id == id))
                    throw new Exception("Threadstag with this ID does not exist");

                await _context.ThreadsTags.AddAsync(threadsTag);
                await _context.SaveChangesAsync();

                return threadsTag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
