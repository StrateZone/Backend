using AutoMapper;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IMapper _mapper;

        public PriceService(IPriceRepository priceRepository, IMapper mapper)
        {
            _priceRepository = priceRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<PriceModel>> GetServicePricesAsync(PriceParameters parameters)
        {
            try
            {
                var result = await _priceRepository.GetServicePrices(parameters);
                var prices = _mapper.Map<PagedList<PriceModel>>(result);

                return new PagedList<PriceModel>(prices, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetPriceOfRoomTypeAsync(string roomType)
        {
            try
            {
                var result = await _priceRepository.GetPriceOfRoomTypeAsync(roomType);
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetPriceOfGameTypeAsync(string gameType)
        {
            try
            {
                var result = await _priceRepository.GetPriceOfGameTypeAsync(gameType);
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetPriceOfCourseAsync(int courseId)
        {
            try
            {
                var result = await _priceRepository.GetPriceOfCourseAsync(courseId);
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal> GetPriceOfAppointmentAsync(int appointmentId)
        {
            try
            {
                return await _priceRepository.GetPriceOfAppointmentAsync(appointmentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetMembershipPriceAsync()
        {
            try
            {
                var result = await _priceRepository.GetMembershipPriceAsync();
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetTeachingSalaryAsync()
        {
            try
            {
                var result = await _priceRepository.GetTeachingSalaryAsync();
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetProductPriceByIdAsync(int productId)
        {
            try
            {
                var result = await _priceRepository.GetProductPriceByIdAsync(productId);
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> UpdatePriceAsync(PriceModel priceModel, int id)
        {
            try
            {
                Price price = _mapper.Map<Price>(priceModel);
                var result = await _priceRepository.UpdatePriceAsync(price, id);
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<decimal>> GetDetailedPriceOfTableFromTimeRangeAsync(int tableId, DateTime FromTime, DateTime ToTime)
        {
            try
            {
                return await _priceRepository.GetDetailedPriceOfTableFromTimeRangeAsync(tableId, FromTime, ToTime);
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal> GetPriceOfAppointmentFromAppointmentRequestAsync(int[] tableIds, DateTime FromTime, DateTime ToTime)
        {
            try
            {
                return await _priceRepository.GetPriceOfAppointmentTablesFromTimeRangeAsync(tableIds, FromTime, ToTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal> GetPriceOfTablesAppointmentAsync(TablesAppointmentModel tablesAppointmentModel)
        {
            try
            {
                var tablesAppointment = _mapper.Map<TablesAppointment>(tablesAppointmentModel);
                return await _priceRepository.GetPriceOfTablesAppointmentAsync(tablesAppointment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal> GetPriceOfAppointmentAsync(AppointmentModel appointmentModel)
        {
            try
            {
                var appointment = _mapper.Map<Appointment>(appointmentModel);
                return await _priceRepository.GetPriceOfAppointmentAsync(appointment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<int, decimal>> GetPricesPerHourEachGameTypeAsync()
        {
            try
            {
                return await _priceRepository.GetPricesPerHourEachGameTypeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<string, decimal>> GetPricesPerHourEachRoomTypeAsync()
        {
            try
            {
                return await _priceRepository.GetPricesPerHourEachRoomTypeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> CreatePriceAsync(PriceModel priceModel)
        {
            try
            {
                var price = _mapper.Map<Price>(priceModel);
                var result = await _priceRepository.CreatePriceAsync(price);
            
                return _mapper.Map<PriceModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
