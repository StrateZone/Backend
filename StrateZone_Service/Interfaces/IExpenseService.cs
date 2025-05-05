using StrateZone_Repository.Entities;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseModel> AddAsync(ExpenseRequest expense);
        Task<List<ExpenseModel>> AddRangeAsync(List<ExpenseRequest> expense);
        Task<ExpenseModel> DeleteAsync(int id);
        Task<ExpenseModel> GetByIdAsync(int id);
        Task<PagedList<ExpenseModel>> GetExpensesAsync(ExpenseParameters parameters);
        Task<ExpenseModel> UpdateAsync(ExpenseModel expense, int id);
    }
}
