using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class TablesAppointmentService : ITablesAppointmentService
    {
        private readonly IAppointmentrequestRepository _requestRepository;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;

        public TablesAppointmentService(ITablesAppointmentRepository tablesAppointmentRepository, IMapper mapper, IPriceService priceService, IPaymentService paymentService, IWalletService walletService, ITransactionService transactionService, IAppointmentrequestRepository repository)
        {
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _mapper = mapper;
            _priceService = priceService;
            _paymentService = paymentService;
            _walletService = walletService;
            _transactionService = transactionService;
            _requestRepository = repository;
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentAsync(parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);
            
                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByTableIdAsync(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentResponse>> GetAllTablesAppointmentByAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByAppointmentIdAsync(id);
                return _mapper.Map<List<TablesAppointmentResponse>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentResponse> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetTablesAppointmentByTableIdAndAppointmentIdAsync(tableId, appointmentId);
                return _mapper.Map<TablesAppointmentResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CreateTablesAppointmentAsync(TablesAppointmentModel tablesAppointmentModel)
        {
            try
            {
                tablesAppointmentModel.Price = await _priceService.GetPriceOfTablesAppointmentAsync(tablesAppointmentModel);
                var tablesAppointment = _mapper.Map<TablesAppointment>(tablesAppointmentModel);

                var result = await _tablesAppointmentRepository.CreateTablesAppointmentAsync(tablesAppointment);
                return _mapper.Map<TablesAppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> CreateTablesAppointmentsFromAppointmentAsync(AppointmentModel appointmentModel)
        {
            try
            {
                var appointment = _mapper.Map<Appointment>(appointmentModel);
                var result = await _tablesAppointmentRepository.CreateTablesAppointmentsFromAppointmentAsync(appointment);

                var mappedResult = _mapper.Map<List<TablesAppointmentModel>>(result);

                return mappedResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> DeleteTablesAppointmentAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.DeleteTablesAppointmentAsync(id);
                return _mapper.Map<TablesAppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentResponse> GetByIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetByIdAsync(id);
                return _mapper.Map<TablesAppointmentResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CheckInTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var tablesAppointmentResponse = await GetByIdAsync(tablesAppointmentId);
                var tablesAppointment = _mapper.Map<TablesAppointmentModel>(tablesAppointmentResponse);

                var payment = (await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId))
                            .SingleOrDefault(p => p.UserId == userId) 
                            ?? throw new Exception("No payment was found for this tables appointment.");

                if ((PaymentStatus) Enum.Parse(typeof(PaymentStatus), payment.PaymentStatus) == PaymentStatus.unpaid)
                    throw new Exception($"Check-in failed: Unpaid appointment. Please proceed with the payment first!");

                string errorMessage = (AppointmentStatus) Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status) switch
                {
                    AppointmentStatus.pending => "This appointment hasn't been confirmed.",
                    AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                    AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                    AppointmentStatus.cancelled => "This appointment has been cancelled.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Check-in failed: {errorMessage}");

                if (tablesAppointment.ScheduleTime > DateTime.UtcNow.AddHours(7).AddMinutes(-5))
                    throw new Exception($"Check-in is not yet opened: Check-in only available 5 minutes prior to schedule time!");

                tablesAppointment.Status = AppointmentStatus.checked_in.ToString();

                var result = await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var refundCalculation = await CalculateRefundAmountOnAppointmentCancellation(userId, tablesAppointmentId, DateTime.UtcNow.AddHours(7));

                var tablesAppointment = refundCalculation.TablesAppointmentModel;
                string errorMessage = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status) switch
                {
                    AppointmentStatus.cancelled => "This appointment has already been cancelled.",
                    AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                    AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    AppointmentStatus.incoming => "Can not cancel incoming appointments.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Cancellation failed: {errorMessage}");

                if (refundCalculation.RefundStatus == RefundStatus.cancellation_fail)
                {
                    throw new Exception(refundCalculation.Message);
                }

                if (refundCalculation.RefundStatus == RefundStatus.no_refund)
                {
                    tablesAppointment.Status = AppointmentStatus.cancelled.ToString();
                    return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
                }

                if (refundCalculation.RefundStatus != RefundStatus.no_refund_while_refund_for_invited_user)
                {
                    var userWallet = await _walletService.GetWalletByUserIdAsync(userId);
                    var refundAmount = refundCalculation.RefundAmount;
                    await _walletService.DepositWalletAsync((int)refundAmount, userWallet.WalletId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content =
                            $"Refund on booking cancellation. / " +
                            $"Table Id: {tablesAppointment.TableId}. / " +
                            $"Appointment Id: {tablesAppointment.AppointmentId}. / " +
                            $"Amount: {refundAmount} VND.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = userId,
                        TransactionType = TransactionType.refund,
                    };
                    await _transactionService.SaveTransaction(newTransaction);
                }
                else
                {
                    var refundAmount = refundCalculation.RefundAmount;
                    var bookingPayments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId);
                    var paymentForInvitedUser = bookingPayments.SingleOrDefault(p => p.UserId != userId);
                    var invitedUserWallet = await _walletService.GetWalletByUserIdAsync((int)paymentForInvitedUser.UserId);

                    await _walletService.DepositWalletAsync((int)tablesAppointment.Price, invitedUserWallet.WalletId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content =
                            $"Refund on shared booking cancellation / " +
                            $"Table Id: {tablesAppointment.TableId}. / " +
                            $"Appointment Id: {tablesAppointment.AppointmentId}. / " +
                            $"Amount: {refundAmount} VND.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = paymentForInvitedUser.UserId,
                        TransactionType = TransactionType.refund,
                    };
                    await _transactionService.SaveTransaction(newTransaction);
                }
                
                tablesAppointment.Status = AppointmentStatus.cancelled.ToString();

                var requests = await _requestRepository.GetAppointmentRequestsByTablesAppointmentIdAsync(tablesAppointmentId);
                foreach (var req in requests)
                {
                    req.Status = RequestStatus.cancelled;
                    await _requestRepository.UpdateAppointmentRequestAsync(req, req.Id);
                }

                return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> ForceCancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var refundCalculation = await CalculateRefundAmountOnAppointmentCancellation(userId, tablesAppointmentId, DateTime.UtcNow.AddHours(7));

                var tablesAppointment = refundCalculation.TablesAppointmentModel;
                string errorMessage = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status) switch
                {
                    AppointmentStatus.cancelled => "This appointment has already been cancelled.",
                    AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                    AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Cancellation failed: {errorMessage}");

                if (refundCalculation.RefundStatus == RefundStatus.no_refund || refundCalculation.RefundStatus == RefundStatus.cancellation_fail)
                {
                    tablesAppointment.Status = AppointmentStatus.cancelled.ToString();
                    return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
                }

                if (refundCalculation.RefundStatus != RefundStatus.no_refund_while_refund_for_invited_user)
                {
                    var userWallet = await _walletService.GetWalletByUserIdAsync(userId);
                    var refundAmount = refundCalculation.RefundAmount;
                    await _walletService.DepositWalletAsync((int)refundAmount, userWallet.WalletId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content =
                            $"Refund on booking cancellation. / " +
                            $"Table Id: {tablesAppointment.TableId}. / " +
                            $"Appointment Id: {tablesAppointment.AppointmentId}. / " +
                            $"Amount: {refundAmount} VND.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = userId,
                        TransactionType = TransactionType.refund,
                    };
                    await _transactionService.SaveTransaction(newTransaction);
                }
                else
                {
                    var refundAmount = refundCalculation.RefundAmount;
                    var bookingPayments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId);
                    var paymentForInvitedUser = bookingPayments.SingleOrDefault(p => p.UserId != userId);
                    var invitedUserWallet = await _walletService.GetWalletByUserIdAsync((int)paymentForInvitedUser.UserId);

                    await _walletService.DepositWalletAsync((int)tablesAppointment.Price, invitedUserWallet.WalletId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content =
                            $"Refund on shared booking cancellation / " +
                            $"Table Id: {tablesAppointment.TableId}. / " +
                            $"Appointment Id: {tablesAppointment.AppointmentId}. / " +
                            $"Amount: {refundAmount} VND.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = paymentForInvitedUser.UserId,
                        TransactionType = TransactionType.refund,
                    };
                    await _transactionService.SaveTransaction(newTransaction);
                }

                tablesAppointment.Status = AppointmentStatus.cancelled.ToString();

                var requests = await _requestRepository.GetAppointmentRequestsByTablesAppointmentIdAsync(tablesAppointmentId);
                foreach (var req in requests)
                {
                    req.Status = RequestStatus.cancelled;
                    await _requestRepository.UpdateAppointmentRequestAsync(req, req.Id);
                }

                return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task<TablesAppointmentRefundResponse> CalculateRefundAmountOnAppointmentCancellation(int userId, int tablesAppointmentId, DateTime CancelTime)
        {
            var tablesAppointment = await _tablesAppointmentRepository.GetByIdAsync(tablesAppointmentId)
                                ?? throw new Exception("Tables appointment with this ID does not exist.");

            if (CancelTime < tablesAppointment.CreatedAt) throw new Exception("Cancel time must the later than created_time.");

            var model = _mapper.Map<TablesAppointmentModel>(tablesAppointment);

            DateTime ScheduleTime = tablesAppointment.ScheduleTime;
            DateTime CreatedTime = (DateTime)tablesAppointment.CreatedAt;

            var bookingPayments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId);
            var paymentForUser = bookingPayments.SingleOrDefault(p => p.UserId == userId);
            if (paymentForUser == null || paymentForUser.PaymentStatus == PaymentStatus.unpaid.ToString())
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.no_refund,
                    Message = "No refund. Reason: Appointment is not yet paid.",
                    CancellationTime = CancelTime,
                };
            }

            if (bookingPayments.Any(p => p.UserId != userId && p.PaymentStatus == PaymentStatus.paid.ToString()))
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.no_refund_while_refund_for_invited_user,
                    Message = "No refund. Reason: Cancellation on shared appointment will not be refund.",
                    CancellationTime = CancelTime,
                };
            }

            DateTime TimeGate_BlockAppointmentCancellation = ScheduleTime.AddHours(-1.5f),
                     TimeGate_Refund50_OnCancellation = ScheduleTime.AddHours(-3.5f);

            if (CancelTime >= TimeGate_BlockAppointmentCancellation)
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.cancellation_fail,
                    Message = "Can not cancel appointment 1.5 hours prior to the scheduled time"
                };
            }

            if (CreatedTime >= TimeGate_Refund50_OnCancellation)
            {
                double TimeDiff_CreatedTimeUntilCancellationBlockTime = TimeGate_BlockAppointmentCancellation.Subtract(CreatedTime).TotalHours;

                DateTime TimeGate_Refund100_OnCancellation = CreatedTime.AddHours(TimeDiff_CreatedTimeUntilCancellationBlockTime / 2);

                if (CancelTime < TimeGate_Refund100_OnCancellation)
                {
                    return new TablesAppointmentRefundResponse()
                    {
                        TablesAppointmentModel = model,
                        RefundAmount = (decimal) tablesAppointment.Price,
                        RefundStatus = RefundStatus.refund_100_percentage_of_total,
                        Message = "Refund 100%",
                        CancellationTime = CancelTime,
                        Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                        Cancellation_PartialRefund_TimeGate = TimeGate_Refund100_OnCancellation,
                    };
                }
                else
                {
                    return new TablesAppointmentRefundResponse()
                    {
                        TablesAppointmentModel = model,
                        RefundAmount = (decimal) (tablesAppointment.Price / 2),
                        RefundStatus = RefundStatus.refund_50_percentage_of_total,
                        Message = "Refund 50%",
                        CancellationTime = CancelTime,
                        Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                        Cancellation_PartialRefund_TimeGate = TimeGate_Refund100_OnCancellation,
                    };
                }
            }
            else if (CancelTime < TimeGate_Refund50_OnCancellation)
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = (decimal)tablesAppointment.Price,
                    RefundStatus = RefundStatus.refund_100_percentage_of_total,
                    Message = "Refund 100%",
                    CancellationTime = CancelTime,
                    Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                    Cancellation_PartialRefund_TimeGate = TimeGate_Refund50_OnCancellation,
                };
            }

            return new TablesAppointmentRefundResponse()
            {
                TablesAppointmentModel = model,
                RefundAmount = (decimal)(tablesAppointment.Price / 2),
                RefundStatus = RefundStatus.refund_50_percentage_of_total,
                Message = "Refund 50%",
                CancellationTime = CancelTime,
                Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                Cancellation_PartialRefund_TimeGate = TimeGate_Refund50_OnCancellation,
            };
        }

        public async Task<TablesAppointmentModel> UpdateTablesAppointmentAsync(TablesAppointmentModel appointmentModel, int id)
        {
            try
            {
                var tablesAppointment = _mapper.Map<TablesAppointment>(appointmentModel);
                var result = await _tablesAppointmentRepository.UpdateTablesAppointmentAsync(tablesAppointment, id);

                var mappedResult = _mapper.Map<TablesAppointmentModel>(result);

                return mappedResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentsFromUserByUserId(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsJoinedByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentsInvitedToUserByUserId(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> UpdateStatusForExpiredAndIncomingTablesAppointments()
        {
            try
            {
                return await _tablesAppointmentRepository.UpdateStatusForExpiredAndIncomingTablesAppointments();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests()
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests();
            
                return _mapper.Map<List<TablesAppointmentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
