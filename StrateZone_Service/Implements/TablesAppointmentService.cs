using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class TablesAppointmentService : ITablesAppointmentService
    {
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IMapper _mapper;

        public TablesAppointmentService(ITablesAppointmentRepository tablesAppointmentRepository, IMapper mapper)
        {
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _mapper = mapper;
        }

        public async Task<List<TablesAppointmentModel>> GetAllTablesAppointmentsAsync()
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentAsync();
                return _mapper.Map<List<TablesAppointmentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> GetAllTablesAppointmentByTableIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByTableIdAsync(id);
                return _mapper.Map<List<TablesAppointmentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> GetAllTablesAppointmentByAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByAppointmentIdAsync(id);
                return _mapper.Map<List<TablesAppointmentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetTablesAppointmentByTableIdAndAppointmentIdAsync(tableId, appointmentId);
                return _mapper.Map<TablesAppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CreateTablesAppointment(TablesAppointmentModel tablesAppointmentModel)
        {
            try
            {
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
                return _mapper.Map<List<TablesAppointmentModel>>(result);
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
    }
}
