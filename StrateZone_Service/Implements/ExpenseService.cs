using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
    
        public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<ExpenseModel> AddAsync(ExpenseRequest expense)
        {
            try
            {
                ExpenseModel model = new ExpenseModel()
                {
                    UserId = expense.UserId,
                    SystemId = 1,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    Type = expense.Type,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    TransactionDate = expense.TransactionDate,
                };

                var result = await _expenseRepository.AddAsync(_mapper.Map<Expense>(model));

                return _mapper.Map<ExpenseModel>(result);
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<ExpenseModel>> AddRangeAsync(List<ExpenseRequest> expenses)
        {
            try
            {
                List<ExpenseModel> models = new();

                foreach (var expense in expenses)
                {
                    models.Add(new ExpenseModel()
                    {
                        UserId = expense.UserId,
                        SystemId = 1,
                        Amount = expense.Amount,
                        Description = expense.Description,
                        Type = expense.Type,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        TransactionDate = expense.TransactionDate,
                    });
                }

                var result = await _expenseRepository.AddRangeAsync(_mapper.Map<List<Expense>>(models));

                return _mapper.Map<List<ExpenseModel>>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ExpenseModel> DeleteAsync(int id)
        {
            try
            {
                var result = await _expenseRepository.DeleteAsync(id);

                return _mapper.Map<ExpenseModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ExpenseModel> GetByIdAsync(int id)
        {
            try
            {
                var result = await _expenseRepository.GetByIdAsync(id);

                return _mapper.Map<ExpenseModel>(result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<PagedList<ExpenseModel>> GetExpensesAsync(ExpenseParameters parameters)
        {
            try
            {
                var result = await _expenseRepository.GetExpensesAsync(parameters);
                var mapped = _mapper.Map<PagedList<ExpenseModel>>(result);

                return new PagedList<ExpenseModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ExpenseModel> UpdateAsync(ExpenseModel expense, int id)
        {
            try
            {
                var result = await _expenseRepository.UpdateAsync(_mapper.Map<Expense>(expense), id);

                return _mapper.Map<ExpenseModel>(result);
            }
            catch
            {
                throw;
            }
        }
    }
}
