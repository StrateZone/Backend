using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPaymentService _paymentService;
        private readonly IPriceService _priceService;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IPriceService priceService, IPaymentService paymentService, ITablesAppointmentRepository tablesAppointmentService)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _priceService = priceService;
            _paymentService = paymentService;
            _tablesAppointmentRepository = tablesAppointmentService;
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

        public async Task<PagedList<TransactionModel>> GetTransactionsAsync(TransactionParameters parameters)
        {
            try
            {
                var result = await _transactionRepository.GetAllTransactionsAsync(parameters);
                var mapped = _mapper.Map<PagedList<TransactionModel>>(result);
            
                return new PagedList<TransactionModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TransactionModel>> GetTransactionsAsync(TransactionParameters parameters, TransactionType[] types)
        {
            try
            {
                var result = await _transactionRepository.GetAllTransactionsAsync(parameters, types);
                var mapped = _mapper.Map<PagedList<TransactionModel>>(result);

                return new PagedList<TransactionModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
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

                return new PagedList<TransactionModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TransactionResponse>> GetAnnualReportForTransactionsGroupedByMonth(int year)
        {
            try
            {
                var refunds = await _transactionRepository.GetTransactionsForRefundAsync(year);
                List<TransactionResponse> responses = new();

                var membershipCost = await _priceService.GetMembershipPriceAsync();

                for (int i = 1; i <= 12; ++i)
                {
                    TransactionResponse transactionResponse = new TransactionResponse()
                    { 
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Refund = refunds.Where(r => r.Amount != null && r.CreatedAt.Value.Month == i).Select(r => (decimal) r.Amount).Sum(),
                        Booking = await _tablesAppointmentRepository.GetAllPaidTablesAppointmentWithinAMonthInYearAsync(i, year),
                        MemberShip = (decimal)((await _paymentService.GetMembershipPaymentsWithinAMonthInYearAsync(i, year)).Count() * membershipCost.Price1),
                        Spending = (await _transactionRepository.GetExpensesWithinAMonthInYearAsync(i, year)).Select(e => e.Amount).Sum(),
                        Voucher = 0,
                    };

                    responses.Add(transactionResponse);
                }

                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TransactionModel> SaveTransaction(TransactionModel newTransaction)
        {
            try
            {
                var mapped = _mapper.Map<Transaction>(newTransaction);
                var result = await _transactionRepository.SaveTransaction(mapped);

                return _mapper.Map<TransactionModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
