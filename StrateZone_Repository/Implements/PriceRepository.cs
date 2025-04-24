using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Linq;
using System.Text;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class PriceRepository : IPriceRepository
    {
        private readonly StrateZoneDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        public PriceRepository(StrateZoneDbContext context, IUserRepository userRepository, ITablesAppointmentRepository tablesAppointmentRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _tablesAppointmentRepository = tablesAppointmentRepository;
        }

        public async Task<PagedList<Price>> GetServicePrices(PriceParameters parameters)
        {
            try
            {
                var prices = _context.Prices.Where(p => !p.TeachingSalary && !p.MemberFee && (p.ProductId == null || p.ProductId == 0)).AsQueryable();
                return await PagedList<Price>.ToPagedList(prices, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetPriceOfGameTypeAsync(GameTypeEnum gameType)
        {
            try
            {
                var price = await _context.Prices
                                    .FromSqlRaw(
                                        @"SELECT p.*
                                        FROM public.""prices"" p
                                        JOIN public.""gameTypes"" g ON p.game_type_id = g.type_id
                                        WHERE g.type_name = @gt::game_type 
                                            AND p.member_fee = false AND p.teaching_salary = false
                                        LIMIT 1",
                                        new NpgsqlParameter("@gt", gameType.ToString())
                                        )
                                    .FirstOrDefaultAsync();

                return price;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetPriceOfRoomTypeAsync(RoomType roomType)
        {
            try
            {
                var price = await _context.Prices
                                    .FromSqlRaw(
                                        @"SELECT * FROM prices 
                                        WHERE room_type = @rt::room_type 
                                            AND member_fee = false AND teaching_salary = false 
                                        LIMIT 1",
                                        new NpgsqlParameter("@rt", roomType.ToString())
                                        )
                                    .FirstOrDefaultAsync();

                return price;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetMembershipPriceAsync()
        {
            try
            {
                return await _context.Prices.AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.MemberFee);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetTeachingSalaryAsync()
        {
            try
            {
                return await _context.Prices.AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.TeachingSalary);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetProductPriceByIdAsync(int productId)
        {
            try
            {
                return await _context.Prices.AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.ProductId == productId);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> UpdatePriceAsync(Price price, int id)
        {
            try
            {
                var existingPrice = await _context.Prices.FindAsync(id) ?? throw new Exception("Price with this ID does not exist");

                _context.Entry(existingPrice).State = EntityState.Detached;

                price.Id = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE prices SET ");

                if (price.GameTypeId.HasValue)
                {
                    sql.Append("game_type_id = @game_type_id, ");
                    parameters.Add(new NpgsqlParameter("@game_type_id", price.GameTypeId.Value));
                }

                if (price.ProductId.HasValue)
                {
                    sql.Append("product_id = @product_id, ");
                    parameters.Add(new NpgsqlParameter("@product_id", price.ProductId.Value));
                }

                if (price.CourseId.HasValue)
                {
                    sql.Append("course_id = @course_id, ");
                    parameters.Add(new NpgsqlParameter("@course_id", price.CourseId.Value));
                }

                sql.Append("room_type = @room_type::room_type, ");
                parameters.Add(new NpgsqlParameter("@room_type", price.RoomType.ToString()));

                sql.Append("member_fee = @member_fee, ");
                parameters.Add(new NpgsqlParameter("@member_fee", price.MemberFee));

                sql.Append("teaching_salary = @teaching_salary, ");
                parameters.Add(new NpgsqlParameter("@teaching_salary", price.TeachingSalary));

                if (price.Price1.HasValue)
                {
                    sql.Append("price = @price, ");
                    parameters.Add(new NpgsqlParameter("@price", price.Price1.Value));
                }

                sql.Append("unit = @unit, ");
                parameters.Add(new NpgsqlParameter("@unit", price.Unit));

                if (price.UpdatedAt.HasValue)
                {
                    sql.Append("updated_at = @updated_at::updated_at, ");
                    parameters.Add(new NpgsqlParameter("@updated_at", price.UpdatedAt));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var updatedPrice = await _context.Prices.FindAsync(id);
                return updatedPrice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Price> GetPriceOfCourseAsync(int courseId)
        {
            try
            {
                return await _context.Prices.FirstOrDefaultAsync(p => p.CourseId == courseId);
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal> GetPriceOfAppointmentAsync(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                                            .AsNoTracking()
                                            .Include(a => a.TablesAppointments)
                                                .ThenInclude(ta => ta.Table)
                                                    .ThenInclude(t => t.Room)
                                            .Include(a => a.TablesAppointments)
                                                .ThenInclude(ta => ta.Table)
                                                    .ThenInclude(t => t.GameType)
                                            .SingleOrDefaultAsync(a => a.AppointmentId == appointmentId)
                    ?? throw new KeyNotFoundException("Appointment with this ID was not found");

                var tablesAppointments = await _context.TablesAppointments
                    .Where(ta => ta.AppointmentId == appointment.AppointmentId)
                    .ToListAsync();

                if (!tablesAppointments.Any())
                    throw new Exception($"No tables found for Appointment ID {appointment.AppointmentId}");

                decimal totalAppointmentPrice = 0;

                foreach (var tablesAppointment in tablesAppointments)
                {
                    decimal tableAppointmentPrice = await GetPriceOfTablesAppointmentAsync(tablesAppointment);
                    totalAppointmentPrice += tableAppointmentPrice;
                }

                return totalAppointmentPrice;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating appointment price: {ex.Message}", ex);
            }
        }

        public async Task<decimal> GetPriceOfTablesAppointmentAsync(TablesAppointment tablesAppointment)
        {
            try
            {
                Console.WriteLine("HELLO CHAT");

                Appointment appointment = await _context.Appointments.FindAsync(tablesAppointment.AppointmentId)
                    ?? throw new Exception($"Appointment with ID {tablesAppointment.AppointmentId} does not exist");

                if (tablesAppointment.Price != null) return (decimal) tablesAppointment.Price;

                decimal DurationInHours = (decimal)tablesAppointment.EndTime.Subtract(tablesAppointment.ScheduleTime).TotalHours;

                var table = await _context.Tables
                                           .Where(t => t.TableId == tablesAppointment.TableId)
                                           .AsNoTracking()
                                           .Include(t => t.GameType)
                                           .Include(t => t.Room)
                                           .FirstOrDefaultAsync()
                            ?? throw new KeyNotFoundException("No tables found with the provided IDs.");

                var roomPrice = await GetPriceOfRoomTypeAsync(table.Room.Type);
                var gamePrice = await GetPriceOfGameTypeAsync(table.GameType.TypeName);

                tablesAppointment.Price = (decimal)((roomPrice.Price1 + gamePrice.Price1) * DurationInHours);

                decimal totalPrice = (decimal)tablesAppointment.Price;

                if (await _userRepository.FindUserAcceptedToJoinTablesAppointment(tablesAppointment) != null) totalPrice /= 2;

                return totalPrice;
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal> GetPriceOfAppointmentAsync(Appointment appointment)
        {
            try
            {
                if (appointment == null)
                    throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null");

                var tablesAppointments = await _context.TablesAppointments
                    .Where(ta => ta.AppointmentId == appointment.AppointmentId)
                    .ToListAsync();

                if (!tablesAppointments.Any())
                    throw new Exception($"No tables found for Appointment ID {appointment.AppointmentId}");

                decimal totalAppointmentPrice = 0;

                foreach (var tablesAppointment in tablesAppointments)
                {
                    decimal tableAppointmentPrice = await GetPriceOfTablesAppointmentAsync(tablesAppointment);
                    totalAppointmentPrice += tableAppointmentPrice;
                }

                return totalAppointmentPrice;
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal> GetPriceOfAppointmentTablesFromTimeRangeAsync(int[] tableIds, DateTime FromTime, DateTime ToTime)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<decimal>> GetDetailedPriceOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime)
        {
            try
            {
                if (FromTime > ToTime) (FromTime, ToTime) = (ToTime, FromTime);

                decimal DurationInHours = (decimal) ToTime.Subtract(FromTime).TotalHours;

                var table = await _context.Tables
                                        .AsNoTracking()
                                        .Include(t => t.GameType)
                                        .Include(t => t.Room)
                                        .SingleOrDefaultAsync(t => t.TableId == tableId)
                        ?? throw new KeyNotFoundException("Table with this ID does not exist");

                var roomPrice = await GetPriceOfRoomTypeAsync(table.Room.Type);
                var gamePrice = await GetPriceOfGameTypeAsync(table.GameType.TypeName);

                return [
                    (decimal)(gamePrice.Price1),
                    (decimal)(roomPrice.Price1),
                    DurationInHours,
                    (decimal) ((roomPrice.Price1 + gamePrice.Price1) * DurationInHours)
                ];
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<int, decimal>> GetPricesPerHourEachGameTypeAsync()
        {
            try
            {
                var prices = await _context.Prices.AsNoTracking()
                    .Where(p => p.GameTypeId != null)
                    .GroupBy(p => p.GameTypeId.Value)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Average(p => (decimal) p.Price1)
                    );

                return prices;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> GetPricesPerHourEachRoomTypeAsync()
        {
            try
            {
                var prices = await _context.Prices.AsNoTracking()
                    .Where(p => p.RoomType.HasValue)
                    .GroupBy(p => p.RoomType) // group by nullable
                    .ToDictionaryAsync(
                        g => g.Key!.Value.ToString(), // safe because of the HasValue filter
                        g => g.Average(p => (decimal)p.Price1)
                    );

                return prices;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving prices: {ex.Message}");
            }
        }
    }
}
