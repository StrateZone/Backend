using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByThreadIdAsync(int id);
        Task<List<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<Comment> GetCommentById(int id);
        Task<Comment> PostCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment, int id);
    }
}
