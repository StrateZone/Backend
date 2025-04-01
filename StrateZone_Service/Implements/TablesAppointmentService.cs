using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
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
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;

        public TablesAppointmentService(ITablesAppointmentRepository tablesAppointmentRepository, IMapper mapper, IPriceService priceService, IPaymentService paymentService)
        {
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _mapper = mapper;
            _priceService = priceService;
            _paymentService = paymentService;
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
                    AppointmentStatus.cancelled => "This appointment has been cancelled.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Check-in failed: {errorMessage}");

                if (tablesAppointment.ScheduleTime > DateTime.UtcNow.AddHours(7).AddMinutes(-5))
                    throw new Exception($"Check-in is not yet opened: Check-in only available 5 minutes prior to schedule time!");

                tablesAppointment.Status = AppointmentStatus.completed.ToString();

                var result = await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<TablesAppointmentModel> CancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            throw new NotImplementedException();
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
    }
}
