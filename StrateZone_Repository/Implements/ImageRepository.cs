using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System.Threading;

namespace StrateZone_Repository.Implements
{
    public class ImageRepository : IImageRepository
    {
        private readonly StrateZoneDbContext _context;

        public ImageRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Image> GetUserAvatarAsync(int userId)
        {
            try
            {
                return await _context.Images.AsNoTracking()
                                .OrderByDescending(i => i.CreatedAt)
                                .FirstOrDefaultAsync(i => i.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> GetEventThumbnailAsync(int eventId)
        {
            try
            {
                return await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.EventId == eventId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> GetTournamentThumbnailAsync(int tournamentId)
        {
            try
            {
                return await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.TournamentId == tournamentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Image>> GetProductImagesAsync(int productId)
        {
            try
            {
                return await _context.Images
                                    .Where(i => i.ProductId == productId)
                                    .OrderBy(i => i.CreatedAt)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> GetThreadImagesAsync(int threadId)
        {
            try
            {
                return await _context.Images
                                    .AsNoTracking()
                                    .Where(i => i.ThreadId == threadId)
                                    .OrderByDescending(i => i.CreatedAt)
                                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> GetGametypeThumbnailAsync(int gametypeId)
        {
            try
            {
                return await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.GameTypeId == gametypeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> CreateImageAsync(Image image)
        {
            try
            {
                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();

                return image;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> UpdateImageAsync(Image image, int id)
        {
            try
            {
                if (await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.ImageId == id) == null)
                    throw new Exception("Image with this ID does not exist");

                image.ImageId = id;

                _context.Images.Update(image);
                await _context.SaveChangesAsync();

                return image;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Image> DeleteImageAsync(int id)
        {
            try
            {
                var toDelete = await _context.Images.FindAsync(id) ?? throw new Exception("Image with this ID does not exist");

                _context.Images.Remove(toDelete);
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
