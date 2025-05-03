using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Threading;
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
                var existing = (await _context.Threads.AsNoTracking().SingleOrDefaultAsync(t => t.ThreadId == id)) ??
                    throw new Exception("Thread with this ID does not eixst");

                thread.CreatedAt = existing.CreatedAt;
                thread.Rating = existing.Rating;
                thread.CreatedBy = existing.CreatedBy;
                thread.ThreadId = id;
                thread.Comments = null;
                thread.ThreadsTags = null;
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

                if (toDelete.Status == PostgreEnums.ThreadStatus.deleted)
                    throw new Exception("This thread is already deleted");

                if (toDelete.Status == PostgreEnums.ThreadStatus.drafted)
                {
                    _context.Threads.Remove(toDelete);
                }
                else
                {
                    toDelete.ThreadId = id;
                    toDelete.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                    toDelete.CreatedByNavigation = null;
                    toDelete.Status = PostgreEnums.ThreadStatus.deleted;

                    _context.Threads.Update(toDelete);
                }
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
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .OrderByDescending(t => t.CreatedAt)
                                .Select(t => new Thread
                                {
                                    ThreadId = t.ThreadId,
                                    CreatedBy = t.CreatedBy,
                                    Title = t.Title,
                                    ThumbnailUrl = t.ThumbnailUrl,
                                    Rating = t.Rating,
                                    Status = t.Status,
                                    CreatedAt = t.CreatedAt,
                                    UpdatedAt = t.UpdatedAt,
                                    Comments = t.Comments,
                                    CreatedByNavigation = t.CreatedByNavigation,
                                    Images = t.Images,
                                    Likes = t.Likes,
                                    ThreadsTags = t.ThreadsTags
                                })
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    "popularity" => threads.OrderByDescending(a => a.CreatedByNavigation.UserLabel == PostgreEnums.UserLabel.top_contributor ? (10 + (a.Comments.Count * 3 + a.Likes.Count) * 1.5) : (a.Comments.Count * 3 + a.Likes.Count)),
                    _ => threads.OrderByDescending(t => t.CreatedAt)
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
                                .Where(t => statuses.Count() <= 0 || statuses.Contains(t.Status) && t.Status != PostgreEnums.ThreadStatus.deleted)
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .OrderByDescending(t => t.CreatedAt)
                                .Select(t => new Thread
                                {
                                    ThreadId = t.ThreadId,
                                    CreatedBy = t.CreatedBy,
                                    Title = t.Title,
                                    ThumbnailUrl = t.ThumbnailUrl,
                                    Rating = t.Rating,
                                    Status = t.Status,
                                    CreatedAt = t.CreatedAt,
                                    UpdatedAt = t.UpdatedAt,
                                    Comments = t.Comments,
                                    CreatedByNavigation = t.CreatedByNavigation,
                                    Images = t.Images,
                                    Likes = t.Likes,
                                    ThreadsTags = t.ThreadsTags
                                })
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    "popularity" => threads.OrderByDescending(a => a.CreatedByNavigation.UserLabel == PostgreEnums.UserLabel.top_contributor ? (10 + (a.Comments.Count * 3 + a.Likes.Count) * 1.5) : (a.Comments.Count * 3 + a.Likes.Count)),
                    _ => threads.OrderByDescending(t => t.CreatedAt)
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Thread>> GetAllThreadsByStatusesAndTagsAsync(ThreadParameters parameters)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Where(t => (parameters.statuses.Length <= 0 || parameters.statuses.Contains(t.Status))
                                        && (parameters.TagIds.Count <= 0 || parameters.TagIds.All(tagId => t.ThreadsTags.Any(tt => tt.TagId == tagId)))
                                        && (parameters.Search == string.Empty || t.Title.Contains(parameters.Search) || t.Content.Contains(parameters.Search))
                                )
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .Select(t => new Thread
                                {
                                    ThreadId = t.ThreadId,
                                    CreatedBy = t.CreatedBy,
                                    Title = t.Title,
                                    ThumbnailUrl = t.ThumbnailUrl,
                                    Rating = t.Rating,
                                    Status = t.Status,
                                    CreatedAt = t.CreatedAt,
                                    UpdatedAt = t.UpdatedAt,
                                    Comments = t.Comments,
                                    CreatedByNavigation = t.CreatedByNavigation,
                                    Images = t.Images,
                                    Likes = t.Likes,
                                    ThreadsTags = t.ThreadsTags
                                })
                                .AsQueryable();

                if (parameters.OrderBy == "friends" && parameters.userId != null)
                {
                    HashSet<int?> friendIds = _context.Friendlists.AsNoTracking()
                                            .Where(f => f.UserId == parameters.userId || f.FriendId == parameters.userId)
                                            .Select(f => f.UserId == parameters.userId ? f.FriendId : f.UserId)
                                            .ToHashSet();
                    threads = threads.Where(t => friendIds.Contains(t.CreatedBy))
                                    .OrderByDescending(t => t.CreatedAt);
                }

                threads = threads.OrderBy(t =>
                        t.ThreadsTags.Any(tt => tt.Tag.TagName == "quan trọng") ? 0 :
                        t.ThreadsTags.Any(tt => tt.Tag.TagName == "thông báo") ? 1 : 2);

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    "popularity" => threads.OrderByDescending(a => a.CreatedByNavigation.UserLabel == PostgreEnums.UserLabel.top_contributor ? (10 + (a.Comments.Count * 3 + a.Likes.Count) * 1.5) : (a.Comments.Count * 3 + a.Likes.Count)),
                    _ => threads.OrderByDescending(t => t.CreatedAt)
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Thread>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, int id)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Where(t => t.CreatedBy == id && t.Status != PostgreEnums.ThreadStatus.deleted)
                                .Include(t => t.CreatedByNavigation)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
                    "likes-count" => threads.OrderBy(a => a.Likes.Count),
                    "likes-count-desc" => threads.OrderByDescending(a => a.Likes.Count),
                    "comments-count" => threads.OrderBy(a => a.Comments.Count),
                    "comments-count-desc" => threads.OrderByDescending(a => a.Comments.Count),
                    "popularity" => threads.OrderByDescending(a => a.Comments.Count * 3 + a.Likes.Count),
                    _ => threads.OrderByDescending(t => t.CreatedAt)
                };

                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(int, bool, int)> GetThreadLikeAndCommentCount(int id)
        {
            var likes = await _context.Likes.AsNoTracking().Where(l => l.ThreadId == id).ToListAsync();
            var cmtCount = await _context.Comments.AsNoTracking().CountAsync(cc => cc.ThreadId == id);

            return (likes.Count, likes.Any(l => l.UserId == id), cmtCount);
        }

        public async Task<PagedList<Thread>> GetThreadsByUserIdAsync(TablesAppointmentParameters parameters, PostgreEnums.ThreadStatus[] statuses, int id)
        {
            try
            {
                var threads = _context.Threads.AsNoTracking()
                                .Where(t => t.CreatedBy == id && statuses.Contains(t.Status)
                                        && t.Status != PostgreEnums.ThreadStatus.deleted)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .AsQueryable();

                threads = parameters.OrderBy switch
                {
                    "created-at" => threads.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => threads.OrderByDescending(a => a.CreatedAt),
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
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.User)
                                .Include(t => t.ThreadsTags)
                                    .ThenInclude(tt => tt.Tag)
                                .SingleOrDefaultAsync(t => t.ThreadId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Thread> GetThreadByIdForAdminDeleteAsync(int id)
        {
            try
            {
                return await _context.Threads.AsNoTracking()
                                .SingleOrDefaultAsync(t => t.ThreadId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Thread>> GetThreadsWithinMonthAsync(int month, int year)
        {
            try
            {
                return await _context.Threads.AsNoTracking()
                                        .Where(u => u.Status != PostgreEnums.ThreadStatus.drafted 
                                            && u.CreatedAt.HasValue
                                            && u.CreatedAt.Value.Year == year
                                            && u.CreatedAt.Value.Month == month)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
