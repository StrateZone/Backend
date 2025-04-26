using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Implements
{
    public class ProfanityRepository : IProfanityRepository
    {
        private readonly StrateZoneDbContext _context;

        public ProfanityRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Profanity>> GetAllAsync(TablesAppointmentParameters parameters, string? searchValue)
        {
            searchValue = searchValue?.ToLower();

            var result = _context.Profanities.AsNoTracking()
                                    .Where(p => searchValue == null || p.Word.ToLower().Contains(searchValue))
                                    .AsQueryable();
            return await PagedList<Profanity>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Profanity?> GetByIdAsync(int id)
        {
            return await _context.Profanities.FindAsync(id);
        }

        public async Task<bool> CheckContain(string content)
        {
            var wordSeparators = new[] { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r', '\t' };

            HashSet<string> words = content
                .ToLower()
                .Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();

            return await _context.Profanities
                .AsNoTracking()
                .AnyAsync(p => words.Contains(p.Word));
        }

        public async Task<Profanity> AddAsync(Profanity profanity)
        {
            await _context.Profanities.AddAsync(profanity);
            await _context.SaveChangesAsync();

            return profanity;
        }

        public async Task DeleteAsync(int id)
        {
            var profanity = await GetByIdAsync(id);
            if (profanity != null)
            {
                _context.Profanities.Remove(profanity);
            }

            await _context.SaveChangesAsync();
        }
    }
}
