using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IAppointmentService _appointmentService;
        private readonly ITableService _tableService;
        private readonly ITablesAppointmentService _tablesAppointmentService;
        private readonly IRoomService _roomService;
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

                return new PagedList<PriceModel>(prices, prices.Count, prices.CurrentPage, prices.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PriceModel> GetPriceOfRoomTypeAsync(RoomType roomType)
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

        public async Task<PriceModel> GetPriceOfGameTypeAsync(GameTypeEnum gameType)
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

        public async Task<PriceModel> GetPriceOfAppointmentAsync(int appointmentId)
        {
            try
            {
                throw new NotImplementedException();
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
    }
}
