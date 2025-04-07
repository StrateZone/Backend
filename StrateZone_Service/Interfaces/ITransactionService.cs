using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionModel> GetById(int id); 
        Task<PagedList<TransactionModel>> GetUserTransactionsAsync(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TransactionModel>> GetTransactionsAsync(TransactionParameters parameters);
        Task<TransactionModel> SaveTransaction(TransactionModel newTransaction);
    }
}
