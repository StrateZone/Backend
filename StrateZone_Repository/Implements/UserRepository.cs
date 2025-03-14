using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Data;
using System.Text;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly StrateZoneDbContext _context;

        public UserRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<User>> GetUsersAsync(UserListParameters parameters)
        {
            try
            {
                var users = _context.Users.AsQueryable();
                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
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

        public async Task<PagedList<User>> GetUsersByUsernameAsync(UserListParameters parameters, string username)
        {
            try
            {
                var users = _context.Users
                                    .Where(u => u.Username.ToLower().Contains(username.ToLower()))
                                    .AsQueryable();
                
                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<User>> GetUsersByRanking(UserListParameters parameters, PostgreEnums.Ranking ranking, int up, int down)
        {
            try
            {
                if (up < down)
                {
                    (down, up) = (up, down);
                }

                Ranking minRanking = Enum.GetValues(typeof(Ranking)).Cast<Ranking>().Min();
                Ranking maxRanking = Enum.GetValues(typeof(Ranking)).Cast<Ranking>().Max();

                Ranking upperBound = (Ranking)Math.Clamp((int)ranking + up, (int)minRanking, (int)maxRanking);
                Ranking lowerBound = (Ranking)Math.Clamp((int)ranking - down, (int)minRanking, (int)maxRanking);

                var users = _context.Users
                                    .FromSqlRaw(
                                            @"SELECT * FROM users WHERE ranking >= @r1::ranking 
                                                AND ranking <= @r2::ranking",
                                        new NpgsqlParameter("@r1", lowerBound.ToString()),
                                        new NpgsqlParameter("@r2", upperBound.ToString()))
                                    .AsQueryable();


                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
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

                using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using var cmd = new NpgsqlCommand(@"
                            INSERT INTO users (username, email, phone, password, gender, role, skill_level, status, created_at) 
                            VALUES (@username, @email, @phone, @password, @gender::gender, @role::user_role, @skillLevel::skill_level, @status, @createdAt)
                            RETURNING user_id;", connection);

                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@gender", user.Gender.ToString());
                    cmd.Parameters.AddWithValue("@role", user.UserRole.ToString());
                    cmd.Parameters.AddWithValue("@skillLevel", user.SkillLevel.ToString());
                    cmd.Parameters.AddWithValue("@status", user.Status);
                    cmd.Parameters.AddWithValue("@createdAt", user.CreatedAt ?? DateTime.UtcNow);

                    var newUserId = await cmd.ExecuteScalarAsync();
                    user.UserId = Convert.ToInt32(newUserId);
                }

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

                sql.Append("ranking = @ranking::ranking, ");
                parameters.Add(new NpgsqlParameter("@ranking", updatedUser.Ranking.ToString()));

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

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phoneNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
