using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionModel> GetById(int id); 
        Task<PagedList<TransactionModel>> GetUserTransactionsAsync(int id, TablesAppointmentParameters parameters);
        Task<PagedList<TransactionModel>> GetTransactionsAsync(TransactionParameters parameters);
        Task<PagedList<TransactionModel>> GetTransactionsAsync(TransactionParameters parameters, TransactionType[] types);
        Task<TransactionModel> SaveTransaction(TransactionModel newTransaction);
        Task<TransactionMonthResponse> GetDailyTransactionReportsInAMonth(int month, int year);
        Task<ProfitMonthResponse> GetDailyProfitInAMonth(int month, int year);
    }
}
