using StrateZone_Repository.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly StrateZoneDbContext _context;

        public VoucherRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Voucher>> GetVouchersAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var vouchers = _context.Vouchers
                                    .Where(v => v.Status == PostgreEnums.VoucherStatus.active)
                                    .OrderByDescending(v => v.Value)
                                    .AsQueryable();

                return await PagedList<Voucher>.ToPagedList(vouchers, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            try
            {
                await _context.Vouchers.AddAsync(voucher);
                await _context.SaveChangesAsync();

                return voucher;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<Voucher>> GetSampleVouchersAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var vouchers = _context.Vouchers
                                    .AsNoTracking()
                                    .Where(v => v.Status == PostgreEnums.VoucherStatus.active && v.IsSample)
                                    .OrderByDescending(v => v.Value)
                                    .AsQueryable();

                return await PagedList<Voucher>.ToPagedList(vouchers, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<Voucher>> GetVouchersByUserIdAsync(TablesAppointmentParameters parameters, int userid)
        {
            try
            {
                var vouchers = _context.Vouchers
                                    .Where(v => v.Status == PostgreEnums.VoucherStatus.active
                                            && !v.IsSample && v.UserId == userid)
                                    .OrderByDescending(v => v.Value)
                                    .AsQueryable();

                return await PagedList<Voucher>.ToPagedList(vouchers, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher, int id)
        {
            try
            {
                if (await _context.Vouchers.AsNoTracking().SingleOrDefaultAsync(v => v.VoucherId == id) == null)
                    throw new Exception("Voucher with this ID does not exist");

                voucher.VoucherId = id;
                _context.Vouchers.Update(voucher);
                await _context.SaveChangesAsync();

                return voucher;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Voucher>> UpdateVouchersAsync(List<Voucher> vouchers)
        {
            try
            {
                var ids = vouchers.Select(v => v.VoucherId).ToHashSet();

                var allIds = await _context.Vouchers.AsNoTracking().Select(v => v.VoucherId).ToHashSetAsync();
                
                if (!ids.IsProperSubsetOf(allIds)) throw new Exception("One or more vouchers do not exist.");

                _context.Vouchers.UpdateRange(vouchers);
                await _context.SaveChangesAsync();

                return vouchers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Voucher> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Vouchers.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Voucher> GetVoucherByPaymentid(int paymentId)
        {
            try
            {
                var payment = await _context.Payments.AsNoTracking().SingleOrDefaultAsync(v => v.Id == paymentId)
                    ?? throw new Exception("Payment with this ID does not exist");

                return await _context.Vouchers.FindAsync(payment.VoucherId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Voucher> DeleteAsync(int id)
        {
            try
            {
                var voucher = await _context.Vouchers.FindAsync(id) ??
                    throw new Exception("Voucher with this ID does not exist");

                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();

                return voucher;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Voucher>> GetVoucherByIdsAsync(HashSet<int> voucherIds)
        {
            try
            {
                var vouchers = await _context.Vouchers
                    .AsNoTracking()
                    .Where(v => voucherIds.Contains(v.VoucherId))
                    .ToListAsync();

                var foundIds = vouchers.Select(v => v.VoucherId).ToHashSet();
                if (foundIds.Count != voucherIds.Count || !foundIds.SetEquals(voucherIds))
                    throw new Exception("One or more vouchers do not exist");

                return vouchers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Voucher>> GetAllVouchersUsedInAMonthAsync(int month, int year)
        {
            try
            {
                return await _context.Vouchers.AsNoTracking()
                    .Where(v => v.Status == PostgreEnums.VoucherStatus.used 
                            && v.DayOfUsage.HasValue 
                            && v.DayOfUsage.Value.Month == month && v.DayOfUsage.Value.Year == year)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
