using StrateZone_Repository.Data;

namespace StrateZone_Repository.Implements
{
    public class ThreadRepository
    {
        private readonly StrateZoneDbContext _context;

        public ThreadRepository( StrateZoneDbContext context )
        {
            _context = context;
        }

        public async Task<Entities.Thread> CreateThreadAsync(Entities.Thread thread)
        {
            try
            {
                await _context.Threads.AddAsync(thread);
                await _context.SaveChangesAsync();

                return thread;
            }
            catch ( Exception ex ) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
