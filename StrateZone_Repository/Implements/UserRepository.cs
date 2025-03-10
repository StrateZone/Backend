using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System.Data;
using System.Text;

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

        public async Task<List<User>> GetUsersByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.Where(u => u.Username.ToLower().Contains(username.ToLower())).ToListAsync();
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
                        INSERT INTO users (username, email, phone, password, gender, role, skill_level, status, created_at) 
                        VALUES ({0}, {1}, {2}, {3}, {4}::gender, {5}::user_role, {6}::skill_level, {7}, {8})
                        RETURNING user_id;";  // RETURNING user_id allows getting the new ID

                var newUserId = await _context.Database.ExecuteSqlRawAsync(
                    insertQuery,
                    user.Username,
                    user.Email,
                    user.Phone,
                    user.Password,
                    user.Gender.ToString(),
                    user.UserRole.ToString(),
                    user.SkillLevel.ToString(),
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

        public async Task<User> UpdateUserAsync(User updatedUser, int id)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id) ?? throw new Exception("User with this ID does not exist");

                _context.Entry(existingUser).State = EntityState.Detached;

                updatedUser.UserId = id;

                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE users SET ");

                if (updatedUser.CartId.HasValue)
                {
                    sql.Append("cart_id = @cartId, ");
                    parameters.Add(new NpgsqlParameter("@cartId", updatedUser.CartId.Value));
                }

                if (!string.IsNullOrEmpty(updatedUser.Username))
                {
                    sql.Append("username = @username, ");
                    parameters.Add(new NpgsqlParameter("@username", updatedUser.Username));
                }

                sql.Append("role = @userRole::user_role, ");
                parameters.Add(new NpgsqlParameter("@userRole", updatedUser.UserRole.ToString()));

                if (!string.IsNullOrEmpty(updatedUser.Email))
                {
                    sql.Append("email = @email, ");
                    parameters.Add(new NpgsqlParameter("@email", updatedUser.Email));
                }

                if (!string.IsNullOrEmpty(updatedUser.Phone))
                {
                    sql.Append("phone = @phone, ");
                    parameters.Add(new NpgsqlParameter("@phone", updatedUser.Phone));
                }

                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    sql.Append("password = @password, ");
                    parameters.Add(new NpgsqlParameter("@password", updatedUser.Password));
                }

                if (!string.IsNullOrEmpty(updatedUser.Status))
                {
                    sql.Append("status = @status, ");
                    parameters.Add(new NpgsqlParameter("@status", updatedUser.Status));
                }

                sql.Append("gender = @gender::gender, ");
                parameters.Add(new NpgsqlParameter("@gender", updatedUser.Gender.ToString()));

                sql.Append("skill_level = @skillLevel::skill_level, ");
                parameters.Add(new NpgsqlParameter("@skillLevel", updatedUser.SkillLevel.ToString()));

                if (!string.IsNullOrEmpty(updatedUser.Address))
                {
                    sql.Append("address = @address, ");
                    parameters.Add(new NpgsqlParameter("@address", updatedUser.Address));
                }

                if (!string.IsNullOrEmpty(updatedUser.AvatarUrl))
                {
                    sql.Append("avatar_url = @avatarUrl, ");
                    parameters.Add(new NpgsqlParameter("@avatarUrl", updatedUser.AvatarUrl));
                }

                if (!string.IsNullOrEmpty(updatedUser.Bio))
                {
                    sql.Append("bio = @bio, ");
                    parameters.Add(new NpgsqlParameter("@bio", updatedUser.Bio));
                }

                if (updatedUser.Points.HasValue)
                {
                    sql.Append("points = @points, ");
                    parameters.Add(new NpgsqlParameter("@points", updatedUser.Points.Value));
                }

                updatedUser.UpdatedAt = DateTime.UtcNow;
                sql.Append("updated_at = @updatedAt ");
                parameters.Add(new NpgsqlParameter("@updatedAt", updatedUser.UpdatedAt));

                sql.Append("WHERE user_id = @userId");
                parameters.Add(new NpgsqlParameter("@userId", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var refreshedUser = await _context.Users.FindAsync(id);
                return refreshedUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
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
