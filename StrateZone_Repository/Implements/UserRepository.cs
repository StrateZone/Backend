using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System.Data;

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
                var existingUser = await _context.Users
                    .Where(u => u.Email == user.Email || u.Username == user.Username)
                    .FirstOrDefaultAsync();
                if (existingUser != null) throw new Exception("A user with this email or username already exists");

                string insertQuery = @"
                        INSERT INTO users (username, email, phone, password, gender, role, status, created_at) 
                        VALUES ({0}, {1}, {2}, {3}, {4}::gender, {5}::user_role, {6}, {7})
                        RETURNING user_id;";  // RETURNING user_id allows getting the new ID

                // Execute SQL Raw with parameters
                var newUserId = await _context.Database.ExecuteSqlRawAsync(
                    insertQuery,
                    user.Username,
                    user.Email,
                    user.Phone,
                    user.Password,
                    user.Gender.ToString().ToLower(), // Ensure it's lowercase like PostgreSQL enums
                    user.UserRole.ToString(),
                    user.Status,
                    user.CreatedAt
                );

                user.UserId = newUserId; // Assign the returned ID to the user object
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
