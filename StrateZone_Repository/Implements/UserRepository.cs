using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;

namespace StrateZone_Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly StrateZoneDbContext _context;

        public UserRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                List<User> users = await _context.Users.ToListAsync();
                if (users.FirstOrDefault(u => u.Email == user.Email) != null) throw new Exception("A user with this email already exists");
                else if (users.FirstOrDefault(u => u.Username == user.Username) != null) throw new Exception("A user with this username already exists");

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> UpdateUserAsync(User user, int id)
        {
            try
            {
                if (await _context.Users.FindAsync(id) == null) throw new Exception("User with this ID does not exist");

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> DeleteUserAsync(int id)
        {
            try
            {
                User toRemove = await _context.Users.FindAsync(id) ?? throw new Exception("User with this ID does not exist");

                _context.Users.Remove(toRemove);
                await _context.SaveChangesAsync();

                return toRemove;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
