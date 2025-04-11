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
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetCommentsByThreadIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> PostCommentAsync(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> UpdateCommentAsync(Comment comment, int id)
        {
            throw new NotImplementedException();
        }
    }
}
