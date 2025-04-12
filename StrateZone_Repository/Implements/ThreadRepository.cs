using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using Thread = StrateZone_Repository.Entities.Thread;

namespace StrateZone_Repository.Implements
{
    public class ThreadRepository : IThreadRepository
    {
        private readonly StrateZoneDbContext _context;

        public ThreadRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Thread> CreateThreadAsync(Thread thread)
        {
            try
            {
                await _context.Threads.AddAsync(thread);
                await _context.SaveChangesAsync();

                return thread;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Thread> UpdateThreadAsync(Thread thread, int id)
        {
            try
            {
                if ((await _context.Threads.AsNoTracking().SingleOrDefaultAsync(t => t.ThreadId == id)) == null)
                    throw new Exception("Thread with this ID does not eixst");

                thread.ThreadId = id;
                thread.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                thread.CreatedByNavigation = null;

                _context.Threads.Update(thread);

                await _context.SaveChangesAsync();
                return thread;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Thread> DeleteThreadAsync(int id)
        {
            try
            {
                var toDelete = await _context.Threads.FindAsync(id) ?? throw new Exception("No thread with this ID was found.");

                _context.Threads.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Thread>> GetAllThreadsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.Likes)
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.Likes)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .OrderByDescending(t => t.CreatedAt)
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    _ => threads
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Thread>> GetAllThreadsByStatusesAsync(TablesAppointmentParameters parameters, PostgreEnums.ThreadStatus[] statuses)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Where(t => statuses.Count() <= 0 || statuses.Contains(t.Status))
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.Likes)
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.Likes)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .OrderByDescending(t => t.CreatedAt)
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    _ => threads
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Thread>> GetAllThreadsByStatusesAndTagsAsync(TablesAppointmentParameters parameters, PostgreEnums.ThreadStatus[] statuses, HashSet<int> TagIds, int? userId = null)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Where(t => (statuses.Length <= 0 || statuses.Contains(t.Status))
                                        && (TagIds.Count <= 0 || TagIds.All(tagId => t.ThreadsTags.Any(tt => tt.TagId == tagId)))
                                )
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.Likes)
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.Likes)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .AsQueryable();

                if (parameters.OrderBy == "friends" && userId != null)
                {
                    HashSet<int?> friendIds = _context.Friendlists.AsNoTracking()
                                            .Where(f => f.UserId == userId || f.FriendId == userId)
                                            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                                            .ToHashSet();

                    threads = threads.Where(t => friendIds.Contains(t.CreatedBy))
                                    .OrderByDescending(t => t.CreatedAt);
                }

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    _ => threads.OrderByDescending(t => t.CreatedAt)
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Thread> GetThreadByIdAsync(int id)
        {
            try
            {
                return await _context.Threads.AsNoTracking()
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.Likes)
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.Likes)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .SingleOrDefaultAsync(t => t.ThreadId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
