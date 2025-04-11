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
    public class LikeRepository : ILikeRepository
    {
        private readonly StrateZoneDbContext _context;

        public LikeRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Like> CreateLike(Like like)
        {
            try
            {
                if (like.ThreadId != null
                    &&
                    await _context.Likes.AsNoTracking().AnyAsync(l => l.UserId == like.UserId && l.ThreadId == like.ThreadId))
                    throw new Exception("Like for this thread has already been sent");
                else if (like.CommentId != null
                    &&
                    await _context.Likes.AsNoTracking().AnyAsync(l => l.UserId == like.UserId && l.CommentId == like.CommentId))
                    throw new Exception("Like for this comment has already been sent");

                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();

                return like;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Like> DeleteLike(int id)
        {
            try
            {
                var like = await _context.Likes.FindAsync(id)
                        ?? throw new Exception("Like with this ID does not exist.");

                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();

                return like;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
