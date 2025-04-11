using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentModel>> GetCommentsByThreadIdAsync(int id);
        Task<List<CommentModel>> GetCommentsByUserIdAsync(int userId);
        Task<CommentModel> GetCommentById(int id);
        Task<CommentModel> PostCommentAsync(CommentRequest comment);
        Task<CommentModel> UpdateCommentAsync(CommentModel comment, int id);
        Task<CommentModel> DeleteCommentAsync(int id);
    }
}
