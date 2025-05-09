using AutoMapper;
using StrateZone_Repository.Pagination;
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
        private readonly IVoucherService _voucherService;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IPriceService priceService, IPaymentService paymentService, ITablesAppointmentRepository tablesAppointmentService, IVoucherService voucherService)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _priceService = priceService;
            _paymentService = paymentService;
            _tablesAppointmentRepository = tablesAppointmentService;
            _voucherService = voucherService;
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

        public async Task<TransactionMonthResponse> GetDailyTransactionReportsInAMonth(int month, int year)
        {
            try
            {
                var deposits = await _transactionRepository.GetTransactionsForDepositWithinAMonthAsync(month, year);
                List<TransactionDayResponse> dailyResponses = new();

                var membershipCost = await _priceService.GetMembershipPriceAsync();

                int dayInMonth = DateTime.DaysInMonth(year, month);
                for (int i = 1; i <= dayInMonth; ++i)
                {
                    decimal depositDay = deposits.Where(r => r.Amount != null && r.CreatedAt.Value.Day == i).Select(r => (decimal)r.Amount).Sum();
                    decimal bookingDay = await _tablesAppointmentRepository.GetAllPaidTablesAppointmentWithinADayInYearAsync(i, month, year);
                    decimal membershipDay = (decimal)((await _paymentService.GetMembershipPaymentsWithinADayInYearAsync(i, month, year)) * membershipCost.Price1);

                    dailyResponses.Add(
                        new()
                        {
                            DayOfMonth = i,
                            Deposit = depositDay,
                            Booking = bookingDay,
                            Refund = 0,
                            Spending = 0,
                            MemberShip = membershipDay,
                            Voucher = 0
                        }
                    );
                }

                return new()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalDays = dayInMonth,
                    transactionDayResponses = dailyResponses
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ProfitMonthResponse> GetDailyProfitInAMonth(int month, int year)
        {
            try
            {
                var refunds = await _transactionRepository.GetTransactionsForRefundWithinAMonthAsync(month, year);
                var deposits = await _transactionRepository.GetTransactionsForDepositWithinAMonthAsync(month, year);
                var expenses = await _transactionRepository.GetExpensesWithinAMonthInYearAsync(month, year);
                var vouchers = await _voucherService.GetAllVouchersUsedInAMonthAsync(month, year);

                List<ProfitDailyResponse> dailyResponses = new();

                int dayInMonth = DateTime.DaysInMonth(year, month);
                for (int i = 1; i <= dayInMonth; ++i)
                {
                    decimal depositDay = deposits.Where(r => r.Amount != null && r.CreatedAt.Value.Day == i).Select(r => (decimal)r.Amount).Sum();
                    decimal refundDay = refunds.Where(r => r.Amount != null && r.CreatedAt.Value.Day == i).Select(r => (decimal)r.Amount).Sum();
                    decimal spendingDay = expenses.Where(r => r.CreatedAt.Day == i).Select(r => r.Amount).Sum();
                    decimal voucherDay = vouchers.Where(r => r.DayOfUsage.Value.Day == i).Select(r => r.Value).Sum();

                    dailyResponses.Add(
                        new() 
                        {
                            DayOfMonth = i,
                            Profit = depositDay - refundDay - spendingDay - voucherDay
                        }
                    );
                }

                return new()
                { 
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalDays = dayInMonth,
                    ProfitDailyResponses = dailyResponses
                };
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
