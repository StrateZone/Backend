using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class AppointmentrequestService : IAppointmentrequestService
    {
        private readonly IAppointmentrequestRepository _appointmentRequestRepository;
        private readonly ITablesAppointmentService _tablesAppointmentService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public AppointmentrequestService(IAppointmentrequestRepository appointmentRequestRepository, IMapper mapper, ITablesAppointmentService appointmentService, IPaymentService paymentService)
        {
            _appointmentRequestRepository = appointmentRequestRepository;
            _mapper = mapper;
            _tablesAppointmentService = appointmentService;
            _paymentService = paymentService;
        }

        public async Task<AppointmentrequestModel> CreateAppointmentRequestAsync(AppointmentrequestRequest request)
        {
            try
            {
                DateTime currentTime = DateTime.UtcNow.AddHours(7);
                DateTime appointmentTime = request.StartTime;

                double timeUntilRequestExpiration = 
                    Math.Max(    
                        Math.Min(
                            24 * 2, 
                            appointmentTime.Subtract(currentTime).TotalHours * 0.5f
                            )
                        , 0.5f
                        );

                AppointmentrequestModel model = new()
                {
                    FromUser = request.FromUser,
                    ToUser = request.ToUser,
                    TableId = request.TableId,
                    AppointmentId = request.AppointmentId,
                    Status = PostgreEnums.RequestStatus.pending,
                    ExpireAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc).AddHours(timeUntilRequestExpiration),
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc),
                };

                var appointmentRequest = _mapper.Map<Appointmentrequest>(model);
                var result = await _appointmentRequestRepository.CreateAppointmentRequestAsync(appointmentRequest);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> DeleteAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.DeleteAppointmentRequestAsync(id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> GetAppointmentRequestByIdAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestByIdAsync(id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(int userId, int tablesAppointmentId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(userId, tablesAppointmentId);
                var appointmentRequestModels = _mapper.Map<List<AppointmentrequestModel>>(result);

                return appointmentRequestModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserIdAsync(parameters, userId);
                var appointmentRequestModels = _mapper.Map<PagedList<AppointmentrequestModel>>(result);

                return new PagedList<AppointmentrequestModel>(
                    appointmentRequestModels, result.TotalCount, result.CurrentPage, result.PageSize
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsOfUserByUserIdAsync(parameters, userId);
                var appointmentRequestModels = _mapper.Map<PagedList<AppointmentrequestModel>>(result);

                return new PagedList<AppointmentrequestModel>(appointmentRequestModels, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(int userId, int tableId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(userId, tableId);
                var appointmentRequestModels = _mapper.Map<List<AppointmentrequestModel>>(result);

                return appointmentRequestModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> UpdateAppointmentRequestAsync(AppointmentrequestModel appointmentRequestModel, int id)
        {
            try
            {
                var appointmentRequest = _mapper.Map<Appointmentrequest>(appointmentRequestModel);
                var result = await _appointmentRequestRepository.UpdateAppointmentRequestAsync(appointmentRequest, id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> AcceptAppointmentrequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.AcceptAppointmentrequestAsync(id);

                // in case the request is accepted AFTER the appointment is booked, create a payment for the invited user
                // otherwise, the payment will be automatically created in CreateAppointmentAsync()
                if (result.AppointmentId != null)
                {
                    var tablesAppointment = await _tablesAppointmentService
                            .GetTablesAppointmentByTableIdAndAppointmentIdAsync(result.TableId, (int) result.AppointmentId);

                    await _paymentService.CreatePaymentAsync(new PaymentModel()
                    {
                        UserId = result.ToUser,
                        TablesAppointmentId = tablesAppointment.Id,
                        PaymentStatus = PostgreEnums.PaymentStatus.unpaid,
                        PaymentType = PostgreEnums.PaymentType.appointment,
                        Description = $"Payment for tables appointment {tablesAppointment.Id} (shared with user {result.FromUser})",
                    });

                    var paymentOfTablesAppointmentOwner = (await _paymentService.GetPaymentsByUserIdAsync(result.FromUser))
                                                        .FirstOrDefault(p => p.TablesAppointmentId == tablesAppointment.Id);

                    PaymentModel updatedPayment = new()
                    {
                        Description = $"Payment for tables appointment {tablesAppointment.Id} (shared with user {result.ToUser})",
                        PaymentStatus = paymentOfTablesAppointmentOwner.PaymentStatus,
                        PaymentType = paymentOfTablesAppointmentOwner.PaymentType,
                    };
                    await _paymentService.UpdatePaymentAsync(updatedPayment, paymentOfTablesAppointmentOwner.Id);
                }

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> RejectAppointmentrequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.RejectAppointmentrequestAsync(id);
                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> CancelAllSentRequestFromUserAsync(int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.CancelAllSentRequestFromUserAsync(userId);
                return _mapper.Map<List<AppointmentrequestModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateExpiredAppointmentRequests()
        {
            try
            {
                return await _appointmentRequestRepository.UpdateExpiredAppointmentRequests();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> LinkAppointmentrequestsToAppointmentAsync(AppointmentModel appointmentModel)
        {
            try
            {
                AppointmentRequestParameters parameters = new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue,
                };

                var tableIds = appointmentModel.TablesAppointments.Select(t => t.TableId).ToArray();

                var tablesAppointment = appointmentModel.TablesAppointments;

                var user_requests = (await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserIdAsync(parameters, appointmentModel.UserId))
                                .Where(ar => 
                                    tableIds.Contains(ar.TableId) 
                                    &&
                                    (ar.Status == PostgreEnums.RequestStatus.pending || ar.Status == PostgreEnums.RequestStatus.accepted)
                                )
                                .ToList();

                foreach (var request in user_requests)
                {
                    request.AppointmentId = appointmentModel.AppointmentId;
                    await _appointmentRequestRepository.UpdateAppointmentRequestAsync(request, request.Id);
                }

                return _mapper.Map<List<AppointmentrequestModel>>(user_requests);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<AppointmentrequestModel> GetAppointmentrequestFromUserToUserInTableAsync(int fromUserId, int toUserId, int tableId)
        {
            throw new NotImplementedException();
        }
    }
}
