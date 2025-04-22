using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly StrateZoneDbContext _context;

        public TransactionRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Transaction>> GetAllTransactionsAsync(TransactionParameters parameters)
        {
            try
            {
                var query = _context.Transactions
                                    .AsNoTracking()
                                    .Include(t => t.OfUserNavigation)
                                    .AsQueryable();

                // filter type
                if (parameters.Type == "user")
                {
                    query = query.Where(t => t.OfUser != null);
                }
                else if (parameters.Type == "system")
                {
                    query = query.Where(t => t.OfUser == null);
                }

                if (!string.IsNullOrWhiteSpace(parameters.SearchValue))
                {
                    string search = parameters.SearchValue.Trim().ToLower();

                    query = query.Where(t =>
                        t.Id.ToString().ToLower().Contains(search) ||
                        t.OfUserNavigation.Email.ToLower().Contains(search));
                }

                query = query.OrderByDescending(t => t.CreatedAt);

                return await PagedList<Transaction>.ToPagedList(query, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Transaction>> GetAllTransactionsAsync(TransactionParameters parameters, TransactionType[] types)
        {
            try
            {
                var query = _context.Transactions
                                    .AsNoTracking()
                                    .Where(t => types.Length <= 0 || types.Contains(t.TransactionType))
                                    .Include(t => t.OfUserNavigation).AsQueryable();

                // filter type
                if (parameters.Type == "user")
                {
                    query = query.Where(t => t.OfUser != null);
                }
                else if (parameters.Type == "system")
                {
                    query = query.Where(t => t.OfUser == null);
                }

                if (!string.IsNullOrWhiteSpace(parameters.SearchValue))
                {
                    string search = parameters.SearchValue.Trim().ToLower();

                    query = query.Where(t =>
                        t.Id.ToString().ToLower().Contains(search) ||
                        t.OfUserNavigation.Email.ToLower().Contains(search));
                }

                query = query.OrderByDescending(t => t.CreatedAt);

                return await PagedList<Transaction>.ToPagedList(query, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Transactions.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Expense>> GetExpensesWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                var result = await _context.Expenses.AsNoTracking().Where(e => e.CreatedAt.Month == month && e.CreatedAt.Year == year).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Transaction>> GetTransactionsForRefundAsync(int year)
        {
            try
            {
                return await _context.Transactions
                                    .AsNoTracking()
                                    .Where(t => t.TransactionType == TransactionType.refund
                                            && t.CreatedAt.HasValue 
                                            && t.CreatedAt.Value.Year == year
                                    )
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Transaction>> GetUsersTransactionsAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = _context.Transactions.OrderByDescending(t => t.CreatedAt).Where(t => t.OfUser == id).AsQueryable();

                return await PagedList<Transaction>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Transaction> SaveTransaction(Transaction transaction)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO transactions (of_user, reference_id, content, amount, type, created_at) 
                    VALUES (@of_user, @reference_id, @content, @amount, @transaction_type::transaction_type, @created_at)
                    RETURNING id;";

                cmd.Parameters.Add(new NpgsqlParameter("@of_user", transaction.OfUser != null ? transaction.OfUser : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@reference_id", transaction.ReferenceId != null ? transaction.ReferenceId : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@content", transaction.Content != null ? transaction.Content : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@amount", transaction.Amount));
                cmd.Parameters.Add(new NpgsqlParameter("@transaction_type", transaction.TransactionType.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@created_at", DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newPaymentId = await cmd.ExecuteScalarAsync();
                transaction.Id = Convert.ToInt32(newPaymentId);

                return transaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
