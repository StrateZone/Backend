using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> SaveTransaction(Transaction transaction);
        Task<Transaction> GetByIdAsync(int id);
        Task<List<Transaction>> GetTransactionsForRefundWithinAYearAsync(int year);
        Task<List<Transaction>> GetTransactionsForRefundWithinAMonthAsync(int month, int year);
        Task<List<Transaction>> GetTransactionsForDepositWithinAYearAsync(int year);
        Task<List<Transaction>> GetTransactionsForDepositWithinAMonthAsync(int month, int year);
        Task<List<Expense>> GetExpensesWithinAMonthInYearAsync(int month, int year);
        Task<PagedList<Transaction>> GetAllTransactionsAsync(TransactionParameters parameters);
        Task<PagedList<Transaction>> GetAllTransactionsAsync(TransactionParameters parameters, TransactionType[] types);
        Task<PagedList<Transaction>> GetUsersTransactionsAsync(int id, TablesAppointmentParameters parameters);
    }
}
