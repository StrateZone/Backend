using StrateZone_Repository.Pagination;
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
                var users = _context.Users.AsNoTracking().AsQueryable();
                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<User>> GetUsersManagementAsync(UserListManagementParameters parameters)
        {
            try
            {
                var users = _context.Users.AsNoTracking().AsQueryable();

                if (parameters.Type == "user")
                {
                    users = users.Where(u => u.UserRole == UserRole.RegisteredUser || u.UserRole == UserRole.Member);
                }
                else if (parameters.Type == "staff_admin")
                {
                    users = users.Where(u => u.UserRole == UserRole.Staff || u.UserRole == UserRole.Admin);
                }
                else if (parameters.Type == "member")
                {
                    users = users.Where(u => u.UserRole == UserRole.Member);
                }

                if (!string.IsNullOrWhiteSpace(parameters.SearchValue))
                {
                    string search = parameters.SearchValue.Trim().ToLower();

                    users = users.Where(u =>
                        u.UserId.ToString().ToLower().Contains(search) ||
                        u.Email.ToLower().Contains(search) ||
                        u.Username.ToLower().Contains(search) ||
                        u.FullName.ToLower().Contains(search));
                }
                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetUsersDashboardAsync()
        {
            try
            {
                var users = _context.Users.AsNoTracking().AsQueryable().ToList();
                return users;
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
                return await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserId == id);
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
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
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
                                    .AsNoTracking()
                                    .Where(u => u.Username.ToLower().Contains(username.ToLower()))
                                    .AsQueryable();

                return await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(PagedList<User>, HashSet<int>, HashSet<int>)> SearchForFriendsByUsernameAsync(UserListParameters parameters, int id, string? username)
        {
            try
            {
                var userFriendIds = await _context.Friendlists.AsNoTracking()
                                                        .Where(f => f.UserId == id || f.FriendId == id)
                                                        .Select(f => (int)(f.UserId == id ? f.FriendId : f.UserId))
                                                        .ToHashSetAsync();

                var userWithFriendRequestIds = await _context.Friendrequests.AsNoTracking()
                                                        .Where(f => (f.FromUser == id || f.ToUser == id) && f.Status == RequestStatus.pending)
                                                        .Select(f => f.FromUser == id ? f.ToUser : f.FromUser)
                                                        .ToHashSetAsync();

                var users = _context.Users
                                    .AsNoTracking()
                                    .Where(u => u.UserId != id && u.Status == UserStatus.Active && (username == null || u.Username.ToLower().Contains(username.ToLower())))
                                    .AsQueryable();

                return (
                            await PagedList<User>.ToPagedList(users, parameters.PageNumber, parameters.PageSize), 
                            userFriendIds, 
                            userWithFriendRequestIds
                        );
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
                Ranking minRanking = Enum.GetValues(typeof(Ranking)).Cast<Ranking>().Min();
                Ranking maxRanking = Enum.GetValues(typeof(Ranking)).Cast<Ranking>().Max();

                Ranking upperBound = (Ranking)Math.Clamp((int)ranking + up, (int)minRanking, (int)maxRanking);
                Ranking lowerBound = (Ranking)Math.Clamp((int)ranking - down, (int)minRanking, (int)maxRanking);

                var users = _context.Users
                                    .FromSqlRaw(
                                            @"SELECT * FROM users
                                                WHERE ranking >= @r1::ranking 
                                                AND ranking <= @r2::ranking",
                                        new NpgsqlParameter("@r1", lowerBound.ToString()),
                                        new NpgsqlParameter("@r2", upperBound.ToString()))
                                    .AsNoTracking()
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
                var existingUsers = await _context.Users.ToListAsync();

                foreach (var existingUser in existingUsers)
                {
                    if (existingUser.Email == user.Email) throw new Exception("A user with this email already exists");
                    else if (existingUser.Username == user.Username) throw new Exception("A user with this username already exists");
                    else if (existingUser.Phone == user.Phone) throw new Exception("A user with this phone already exists");
                }

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                        INSERT INTO users (username, email, phone, full_name, password, gender, role, skill_level, label, status, created_at, is_hashed_password) 
                        VALUES (@username, @email, @phone, @fullname, @password, @gender::gender, @role::user_role, @skillLevel::skill_level, @label, @status, @createdAt, true)
                        RETURNING user_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@username", user.Username));
                cmd.Parameters.Add(new NpgsqlParameter("@email", user.Email));
                cmd.Parameters.Add(new NpgsqlParameter("@fullname", user.FullName));
                cmd.Parameters.Add(new NpgsqlParameter("@phone", user.Phone ?? (object)DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@password", user.Password));
                cmd.Parameters.Add(new NpgsqlParameter("@gender", user.Gender.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@role", user.UserRole.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@skillLevel", user.SkillLevel.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@label", user.UserLabel.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@status", user.Status.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@createdAt", user.CreatedAt ?? DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newUserId = await cmd.ExecuteScalarAsync();
                user.UserId = Convert.ToInt32(newUserId);

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
                var existingUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserId == id) ?? throw new Exception("User with this ID does not exist");

                if (await _context.Users.AsNoTracking().AnyAsync(u => u != existingUser && u.Username == updatedUser.Username))
                    throw new Exception("Duplicated username detected.");

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

                if (!string.IsNullOrEmpty(updatedUser.FullName))
                {
                    sql.Append("full_name = @fullname, ");
                    parameters.Add(new NpgsqlParameter("@fullname", updatedUser.FullName));
                }

                sql.Append("status = @status, ");
                parameters.Add(new NpgsqlParameter("@status", updatedUser.Status.ToString()));

                if (updatedUser.ContributionPoints.HasValue)
                {
                    sql.Append("contribution_points = @cp, ");
                    parameters.Add(new NpgsqlParameter("@cp", updatedUser.ContributionPoints.Value));
                }

                sql.Append("label = @user_label, ");
                parameters.Add(new NpgsqlParameter("@user_label", updatedUser.UserLabel.ToString()));

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

                updatedUser.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc);
                sql.Append("updated_at = @updatedAt, ");
                parameters.Add(new NpgsqlParameter("@updatedAt", updatedUser.UpdatedAt));

                if (updatedUser.OTP == null)
                {
                    sql.Append("otp = NULL, ");
                }
                else
                {
                    sql.Append("otp = @OTP, ");
                    parameters.Add(new NpgsqlParameter("@OTP", updatedUser.OTP));
                }

                if (updatedUser.OTPExpiry == null)
                {
                    sql.Append("otp_expiry = NULL, ");
                }
                else
                {
                    sql.Append("otp_expiry = @OTPExpiry, ");
                    parameters.Add(new NpgsqlParameter("@OTPExpiry", updatedUser.OTPExpiry ?? (object)DBNull.Value));
                }

                if (!string.IsNullOrEmpty(updatedUser.RefreshToken))
                {
                    sql.Append("refresh_token = @refreshToken, ");
                    parameters.Add(new NpgsqlParameter("@refreshToken", updatedUser.RefreshToken));
                }

                if (updatedUser.RefreshTokenExpiry != null)
                {
                    sql.Append("refresh_token_expiry = @refreshTokenExpiry, ");
                    parameters.Add(new NpgsqlParameter("@refreshTokenExpiry", updatedUser.RefreshTokenExpiry));
                }

                if (updatedUser.MembershipExpiry != null)
                {
                    sql.Append("membership_expiry = @membershipExpiry, ");
                    parameters.Add(new NpgsqlParameter("@membershipExpiry", updatedUser.MembershipExpiry));
                }

                if (updatedUser.IsPasswordHashed.HasValue)
                {
                    sql.Append("is_hashed_password = @a, ");
                    parameters.Add(new NpgsqlParameter("@a", updatedUser.IsPasswordHashed.Value));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE user_id = @userId");
                parameters.Add(new NpgsqlParameter("@userId", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(updatedUser).State = EntityState.Detached;
                
                return updatedUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }

        public async Task<User> EditUserProfileAsync(User updatedUser, int id)
        {
            try
            {
                var existingUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserId == id) ?? throw new Exception("User with this ID does not exist");

                if (await _context.Users.AsNoTracking().AnyAsync(u => u != existingUser && u.Username == updatedUser.Username))
                    throw new Exception("Duplicated username detected.");

                updatedUser.UserId = id;

                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE users SET ");

                if (!string.IsNullOrEmpty(updatedUser.Username))
                {
                    sql.Append("username = @username, ");
                    parameters.Add(new NpgsqlParameter("@username", updatedUser.Username));
                }

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

                if (!string.IsNullOrEmpty(updatedUser.FullName))
                {
                    sql.Append("full_name = @fullname, ");
                    parameters.Add(new NpgsqlParameter("@fullname", updatedUser.FullName));
                }

                if (updatedUser.ContributionPoints.HasValue)
                {
                    sql.Append("contribution_points = @cp, ");
                    parameters.Add(new NpgsqlParameter("@cp", updatedUser.ContributionPoints.Value));
                }

                sql.Append("gender = @gender::gender, ");
                parameters.Add(new NpgsqlParameter("@gender", updatedUser.Gender.ToString()));

                if (!string.IsNullOrEmpty(updatedUser.Address))
                {
                    sql.Append("address = @address, ");
                    parameters.Add(new NpgsqlParameter("@address", updatedUser.Address));
                }

                if (!string.IsNullOrEmpty(updatedUser.Bio))
                {
                    sql.Append("bio = @bio, ");
                    parameters.Add(new NpgsqlParameter("@bio", updatedUser.Bio));
                }

                updatedUser.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc);
                sql.Append("updated_at = @updatedAt, ");
                parameters.Add(new NpgsqlParameter("@updatedAt", updatedUser.UpdatedAt));

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE user_id = @userId");
                parameters.Add(new NpgsqlParameter("@userId", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                _context.Entry(updatedUser).State = EntityState.Detached;

                return updatedUser;
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
                User toRemove = await _context.Users
                                    .FindAsync(id)
                                    ?? throw new Exception("User with this ID does not exist");

                if (toRemove.Wallet != null) _context.Wallets.Remove(toRemove.Wallet);
                if (toRemove.Cart != null) _context.Carts.Remove(toRemove.Cart);
                _context.Users.Remove(toRemove);
                await _context.SaveChangesAsync();

                return toRemove;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                return user;

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

        public async Task<int> DeleteUnactivatedAccountsAsync(int daysAfterAccountCreate)
        {
            try
            {
                var currentDay = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                var accounts = await _context.Users
                    .Where(a =>
                        a.Status == UserStatus.Unactivated &&
                        (a.CreatedAt == null || ((DateTime)a.CreatedAt).AddDays(daysAfterAccountCreate) < currentDay))
                    .Include(a => a.Wallet)
                    .Include(a => a.Cart)
                    .ToListAsync();

                if (accounts.Count > 0)
                    _context.Users.RemoveRange(accounts);

                await _context.SaveChangesAsync();
                return accounts.Count;
            }
            catch
            {
                throw;
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

        public async Task<User> FindUserAcceptedToJoinTablesAppointment(TablesAppointment tablesAppointment)
        {
            try
            {
                var appointmentRequest =
                        await _context.AppointmentRequests
                                    .FromSqlRaw(
                                        "SELECT * FROM appointment_requests " +
                                        "WHERE table_id = {0} AND appointment_id = {1} " +
                                        "AND status = 'accepted' " +
                                        "LIMIT 1",
                                        tablesAppointment.TableId, 
                                        tablesAppointment.AppointmentId)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync()
                                ?? null;

                if (appointmentRequest == null) return null;

                return await _context.Users.SingleOrDefaultAsync(u => u.UserId == appointmentRequest.ToUser);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<User>, List<User>, HashSet<int>)> GetRandomOpponentsAsync(int userId, string? SearchTerm, HashSet<int> excludedIds)
        {
            try
            {
                var friends = await _context.Friendlists.AsNoTracking()
                                                  .Where(f => (f.UserId == userId && !excludedIds.Contains((int)f.FriendId))
                                                        || (f.FriendId == userId && !excludedIds.Contains((int)f.UserId))
                                                        )
                                                  .Select(f => (int) (f.UserId == userId ? f.FriendId : f.UserId))
                                                  .ToHashSetAsync();

                List<User> randomUsers;

                if (SearchTerm == null)
                {
                    randomUsers = await _context.Users
                                        .AsNoTracking()
                                        .Where(u => u.UserId != userId && !friends.Contains(u.UserId) && !excludedIds.Contains(u.UserId))
                                        .Include(u => u.AppointmentRequestsToUserNavigations)
                                        .OrderByDescending(u => Guid.NewGuid())
                                        .Take(12)
                                        .ToListAsync();

                    return (
                                randomUsers,
                                await _context.Users.AsNoTracking().Where(u => friends.Contains(u.UserId)).ToListAsync(),
                                randomUsers.Select(r => r.UserId).ToHashSet()
                            );
                }
                else
                {
                    randomUsers = await _context.Users
                                        .AsNoTracking()
                                        .Where(u => u.UserId != userId && u.Username.ToLower().Contains(SearchTerm.ToLower()) && !excludedIds.Contains(u.UserId))
                                        .Include(u => u.AppointmentRequestsToUserNavigations)
                                        .OrderByDescending(u => u.Points)
                                        .ToListAsync();

                    return (randomUsers, [], randomUsers.Select(r => r.UserId).ToHashSet());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByAppointmentIdAsync(int id)
        {
            try
            {
                var appointment = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.AppointmentId == id);

                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == appointment.UserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignTopContributorsAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE users
                    SET label = 'none';
                ");

                var maxContributors = (await _context.Systems.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == 1))?.Numberof_TopContributors_PerWeek ?? 0;

                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE users
                    SET label = 'top_contributor'
                    WHERE user_id IN (
                        SELECT user_id
                        FROM users
                        WHERE role = 'Member'
                        ORDER BY contribution_points DESC
                        LIMIT {0}
                    );
                ", maxContributors);

                var usersWithPoints = await _context.Users
                    .Where(u => u.ContributionPoints > 0)
                    .Select(u => new { u.UserId, u.ContributionPoints })
                    .ToListAsync();

                foreach (var user in usersWithPoints)
                {
                    var history = new PointsHistory
                    {
                        OfUser = user.UserId,
                        Amount = -user.ContributionPoints,
                        Content = $"-{user.ContributionPoints} điểm đóng góp: Làm mới điểm mỗi tuần.",
                        PointType = "contribution_point",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                    };

                    await _context.PointsHistories.AddAsync(history);
                }

                await _context.SaveChangesAsync();

                var topContributors = await _context.Users
                    .Where(u => u.UserLabel == UserLabel.top_contributor)
                    .Select(u => u.UserId)
                    .ToListAsync();

                foreach (var userId in topContributors)
                {
                    var notification = new Notification
                    {
                        ToUser = userId,
                        Title = "Chúc mừng! Bạn là một trong những người đóng góp hàng đầu của StrateZone!",
                        Content = "Bạn đã trở thành người đóng góp hàng đầu trong tuần này. " +
                        "Tên và các bài viết của bạn sẽ được hiển thị nổi bật hơn trong cộng đồng, " +
                        "và bạn sẽ được hưởng quyền lợi đổi các voucher với mức ưu đãi.",
                        Type = PostgreEnums.NotificationType.community,
                        Status = MessageStatus.unread,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                    };

                    await _context.Notifications.AddAsync(notification);
                }
                await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE users
                    SET contribution_points = 0;
                ");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to reset top contributors: " + ex.Message, ex);
            }
        }

        public async Task UpdateExpiredMemberships()
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow.AddHours(7);
                var formattedDate = now.ToString("yyyy-MM-dd HH:mm:ss");

                var sql = @"
                            UPDATE users
                            SET role = 'RegisteredUser'
                            WHERE role = 'Member' 
                              AND membership_expiry < {0}";

                await _context.Database.ExecuteSqlRawAsync(sql, now);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to reset user roles: " + ex.Message, ex);
            }
        }

        public async Task<List<User>> GetNewUserWithinDayAsync(int day, int month, int year)
        {
            try
            {
                return await _context.Users.AsNoTracking()
                                        .Where(u => u.CreatedAt.HasValue
                                            && u.CreatedAt.Value.Year == year
                                            && u.CreatedAt.Value.Month == month
                                            && u.CreatedAt.Value.Day == day)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
