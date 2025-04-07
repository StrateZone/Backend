using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> SaveTransaction(Transaction transaction);
        Task<Transaction> GetByIdAsync(int id);
        Task<PagedList<Transaction>> GetAllTransactionsAsync(TransactionParameters parameters);
        Task<PagedList<Transaction>> GetUsersTransactionsAsync(int id, TablesAppointmentParameters parameters);
    }
}
