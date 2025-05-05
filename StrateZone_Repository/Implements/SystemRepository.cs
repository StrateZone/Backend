using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ESystem = StrateZone_Repository.Entities.System;

namespace StrateZone_Repository.Implements
{
    public class SystemRepository : ISystemRepository
    {
        private readonly StrateZoneDbContext _context;

        public SystemRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<ESystem>> GetSystemsAsync()
        {
            try
            {
                return await _context.Systems.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ESystem> GetSystemsByIdAsync(int id)
        {
            try
            {
                return await _context.Systems.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetOpeningHourAsync(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.OpenTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetClosingHourAsync(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.CloseTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetOpeningHourOnDateAsync(int id, DateOnly date)
        {
            try
            {
                var abnormalDay = await _context.AbnormalDays
                                .AsNoTracking()
                                .FirstOrDefaultAsync(d => d.SystemId == id && d.Date == date);

                if (abnormalDay != null) return abnormalDay.OpenTime;

                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.OpenTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetClosingHourOnDateAsync(int id, DateOnly date)
        {
            try
            {
                var abnormalDay = await _context.AbnormalDays
                                .AsNoTracking()
                                .FirstOrDefaultAsync(d => d.SystemId == id && d.Date == date);

                if (abnormalDay != null) return abnormalDay.CloseTime;

                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.CloseTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetAppointmentRefund100TimeInHoursAsync(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.Appointment_Refund100_HoursFromScheduleTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetAppointmentIncomingTimeInHoursAsync(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.Appointment_Incoming_HoursFromScheduleTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetAppointmentCheckinTimeInMinuesAsync(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.Appointment_Checkin_MinutesFromScheduleTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ESystem> UpdateAppointmentTimeRulesAsync(int id, decimal refund100Time, decimal incomingTime, int minutesCheckin, int maxTablesCancelPerWeek)
        {
            try
            {
                var system = await _context.Systems.FindAsync(id) ?? throw new Exception("System with this ID does not exist");

                system.Appointment_Refund100_HoursFromScheduleTime = refund100Time;
                system.Appointment_Incoming_HoursFromScheduleTime = incomingTime;
                system.Appointment_Checkin_MinutesFromScheduleTime = minutesCheckin;
                system.Max_NumberOfTables_CancelPerWeek = maxTablesCancelPerWeek;

                _context.Systems.Update(system);
                await _context.SaveChangesAsync();

                return system;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task<ESystem> UpdatePointsRulesAsync(int id, float userPointsPerCheckedinTableByTablePricesPercentage, int contributionPointsPerThread, int contributionPointsPerComment)
        {
            try
            {
                var system = await _context.Systems.FindAsync(id) ?? throw new Exception("System with this ID does not exist");

                system.UserPoints_PerCheckinTable_ByPercentageOfTablesPrice = userPointsPerCheckedinTableByTablePricesPercentage;
                system.ContributionPoints_PerThread = contributionPointsPerThread;
                system.ContributionPoints_PerComment = contributionPointsPerComment;

                _context.Systems.Update(system);
                await _context.SaveChangesAsync();

                return system;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ESystem> UpdateSystemWorkingHoursAsync(int id, TimeOnly openTime, TimeOnly closeTime)
        {
            try
            {
                var system = await _context.Systems.FindAsync(id) ?? throw new Exception("System with this ID does not exist");

                system.OpenTime = openTime;
                system.CloseTime = closeTime;

                _context.Systems.Update(system);
                await _context.SaveChangesAsync();  

                return system;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AbnormalDay> AddAbnormalDayAsync(AbnormalDay abnormalDay)
        {
            try
            {
                await _context.AbnormalDays.AddAsync(abnormalDay);
                await _context.SaveChangesAsync();

                return abnormalDay;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AbnormalDay> UpdateAbnormalDayAsync(AbnormalDay abnormalDay, int id)
        {
            try
            {
                var day = await _context.AbnormalDays.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id)
                            ?? throw new Exception("Abnormal day with this ID does not exist.");

                abnormalDay.Id = id;
                _context.AbnormalDays.Update(abnormalDay);
                await _context.SaveChangesAsync();

                return abnormalDay;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ESystem> UpdateSystemAsync(ESystem system, int id)
        {
            try
            {
                var day = await _context.Systems.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id)
                            ?? throw new Exception("System with this ID does not exist.");

                system.Id = id;
                _context.Systems.Update(system);
                await _context.SaveChangesAsync();

                return system;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<AbnormalDay>> GetAbnormalDaysAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var days = _context.AbnormalDays.AsNoTracking().Where(a => a.SystemId == id).AsQueryable();

                return await PagedList<AbnormalDay>.ToPagedList(days, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AbnormalDay> DeleteAbnormalDayAsync(int id)
        {
            try
            {
                var abnormalDay = await _context.AbnormalDays.FindAsync(id)
                            ?? throw new Exception("Abnormal day with this ID does not exist.");

                _context.AbnormalDays.Remove(abnormalDay);
                await _context.SaveChangesAsync();

                return abnormalDay;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetMaxNumberOfTablesCancelPerWeek(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.Max_NumberOfTables_CancelPerWeek;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetContributionPointsPerThread(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.ContributionPoints_PerThread;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetContributionPointsPerComment(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.ContributionPoints_PerComment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<float> GetUserPointsPerCheckedInTableInPercentageOfTablesPrice(int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return system.UserPoints_PerCheckinTable_ByPercentageOfTablesPrice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetUserPointsForCheckingInByTablesPrice(decimal price, int id)
        {
            try
            {
                var system = await _context.Systems
                                .AsNoTracking()
                                .SingleOrDefaultAsync(s => s.Id == id);

                return (int) Math.Floor(price * (decimal) system.UserPoints_PerCheckinTable_ByPercentageOfTablesPrice);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetNumberOfTopContributionsPerThread(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.Numberof_TopContributors_PerWeek;
        }

        public async Task<float> GetMaxHoursUntilAppointmentRequestExpiration(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.AppointmentRequest_MaxHours_UntilExpiration;
        }

        public async Task<float> GetMinHoursUntilAppointmentRequestExpiration(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.AppointmentRequest_MinHours_UntilExpiration;
        }

        public async Task<int> GetMaxUsersInvitedToTable(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.Max_NumberOfUsers_InvitedToTable;
        }

        public async Task<float> GetPercentageRefundIfNot100(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.PercentageRefund_IfNot100;
        }

        public async Task<float> GetPercentageTimerangeUntilRequestExpiration(int id)
        {
            var system = await _context.Systems
                            .AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == id);

            return system.PercentageTimeRange_UntilRequestExpiration;
        }
    }
}
