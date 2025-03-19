using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System.Text;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class PriceRepository : IPriceRepository
    {
        private readonly StrateZoneDbContext _context;

        public PriceRepository(StrateZoneDbContext context)
        {
            _context = context;
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
                return await _context.Prices
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
                return await _context.Prices
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
                return await _context.Prices
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

        public async Task<Price> GetPriceOfAppointmentAsync(int appointmentId)
        {
            try
            {
                //var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new Exception("sasa");
                //double appointmentDurationInHours = appointment.EndTime.Subtract(appointment.ScheduleTime).TotalHours;

                throw new NotImplementedException();
            }
            catch
            {
                throw;
            }
        }

        public Task<decimal> GetTotalOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime)
        {
            try
            {
                if (FromTime > ToTime) (FromTime, ToTime) = (ToTime, FromTime);

                double DurationInHours = ToTime.Subtract(FromTime).TotalHours;
                
                throw new NotImplementedException();
            }                                 
            catch
            {
                throw;
            }
        }
    }
}
