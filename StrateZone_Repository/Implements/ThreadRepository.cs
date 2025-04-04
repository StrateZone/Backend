using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Data;
using StrateZone_Repository.Parameters;

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

        public async Task<Entities.Thread> UpdateThreadAsync(Entities.Thread thread, int id)
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

        public async Task<Entities.Thread> DeleteThreadAsync(int id)
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

        public async Task<PagedList<Entities.Thread>> GetAllThreadsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var threads = _context.Threads.AsQueryable();
                return await PagedList<Entities.Thread>.ToPagedList(threads, parameters.PageNumber, parameters.PageSize);
            }
            catch( Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Entities.Thread> GetThreadByIdAsync(int id)
        {
            try
            {
                return await _context.Threads.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
