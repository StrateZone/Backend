using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly StrateZoneDbContext _context;

        public ExpenseRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Expense>> GetExpensesAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var expenses = _context.Expenses.AsNoTracking().
                    Where(e => e.TransactionDate.Month == parameters.Month 
                    && e.TransactionDate.Year == parameters.Year).
                    AsQueryable();

                expenses = parameters.OrderBy switch
                {
                    "created-at" => expenses.OrderBy(a => a.CreatedAt),
                    "created-at-desc" => expenses.OrderByDescending(a => a.CreatedAt),
                    "transaction-date" => expenses.OrderBy(a => a.TransactionDate),
                    "transaction-date-desc" => expenses.OrderByDescending(a => a.TransactionDate),
                    "amount" => expenses.OrderBy(a => a.Amount),
                    "amount-desc" => expenses.OrderByDescending(a => a.Amount),
                    "type" => expenses.OrderBy(a => a.Type),
                    "type-desc" => expenses.OrderByDescending(a => a.Type),
                    _ => expenses.OrderByDescending(t => t.CreatedAt)
                };

                return await PagedList<Expense>.ToPagedList(expenses, parameters.PageNumber, parameters.PageSize);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Expenses.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Expense> AddAsync(Expense expense)
        {
            try
            {
                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();

                return expense;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Expense>> AddRangeAsync(List<Expense> expense)
        {
            try
            {
                await _context.Expenses.AddRangeAsync(expense);
                await _context.SaveChangesAsync();

                return expense;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Expense> UpdateAsync(Expense expense, int id)
        {
            try
            {
                if (!await _context.Expenses.AsNoTracking().AnyAsync(e => e.Id == id))
                    throw new Exception("No expense with this ID was found");

                expense.Id = id;
                _context.Expenses.Update(expense);
                await _context.SaveChangesAsync();

                return expense;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Expense> DeleteAsync(int id)
        {
            try
            {
                var toDelete = (await _context.Expenses.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id)) ??
                    throw new Exception("No expense with this ID was found");

                _context.Expenses.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch
            {
                throw;
            }
        }
    }
}
