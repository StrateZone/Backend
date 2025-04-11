using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;

namespace StrateZone_Repository.Implements
{
    public class CommentRepository : ICommentRepository
    {
        private readonly StrateZoneDbContext _context;

        public CommentRepository( StrateZoneDbContext context )
        {
            _context = context;
        }

        public Task<Comment> GetCommentById(int id)
        {
            try
            {
                return _context.Comments.AsNoTracking()
                                        .Include(c => c.Likes)
                                        .SingleOrDefaultAsync(c => c.CommentId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<Comment>> GetCommentsByThreadIdAsync(int id)
        {
            try
            {
                return _context.Comments.AsNoTracking()
                                        .Where(c => c.ThreadId == id)
                                        .Include(c => c.Likes)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            try
            {
                return _context.Comments.AsNoTracking()
                                        .Where(c => c.UserId == userId)
                                        .Include(c => c.Likes)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Comment> PostCommentAsync(Comment comment)
        {
            try
            {
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                return comment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment, int id)
        {
            try
            {
                if (await _context.Comments.AsNoTracking().SingleOrDefaultAsync(c => c.CommentId == id) == null)
                    throw new Exception("Comment with this ID does not exist");

                comment.CommentId = id;
                comment.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                comment.User = null;

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return comment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Comment> DeleteCommentAsync(int id)
        {
            try
            {
                var toDelete = await _context.Comments.FindAsync(id)
                    ?? throw new Exception("Comment with this ID does not exist");

                _context.Comments.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
