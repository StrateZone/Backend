using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<TransactionModel> GetById(int id)
        {
            try
            {
                var result = await _transactionRepository.GetByIdAsync(id);
                return _mapper.Map<TransactionModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TransactionModel>> GetTransactionsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _transactionRepository.GetAllTransactionsAsync(parameters);
                var mapped = _mapper.Map<PagedList<TransactionModel>>(result);
            
                return new PagedList<TransactionModel>(mapped, result.Count, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TransactionModel>> GetUserTransactionsAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _transactionRepository.GetUsersTransactionsAsync(id, parameters);
                var mapped = _mapper.Map<PagedList<TransactionModel>>(result);

                return new PagedList<TransactionModel>(mapped, result.Count, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
