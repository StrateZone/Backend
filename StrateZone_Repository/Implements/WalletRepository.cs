using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class WalletRepository : IWalletRepository
    {
        private readonly StrateZoneDbContext _context;

        public WalletRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
