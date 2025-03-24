using MealHunt_Repositories.Pagination;
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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly StrateZoneDbContext _context;

        public TransactionRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> SaveTransaction(Transaction transaction)
        {
            try
            {
                var existingTransaction = await _context.Transactions
                    .Where(t => t.OfUser == transaction.OfUser && t.ReferenceId == transaction.ReferenceId)
                    .FirstOrDefaultAsync();
                if (existingTransaction == null)
                {
                    throw new Exception("Existed Transaction");
                }
                var savedTrans = await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                return savedTrans.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
