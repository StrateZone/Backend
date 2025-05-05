using StrateZone_Repository.Entities;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense> AddAsync(Expense expense);
        Task<List<Expense>> AddRangeAsync(List<Expense> expense);
        Task<Expense> DeleteAsync(int id);
        Task<Expense> GetByIdAsync(int id);
        Task<PagedList<Expense>> GetExpensesAsync(ExpenseParameters parameters);
        Task<Expense> UpdateAsync(Expense expense, int id);
    }
}