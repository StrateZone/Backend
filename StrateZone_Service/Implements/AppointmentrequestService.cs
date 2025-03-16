using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class AppointmentrequestService : IAppointmentrequestService
    {
        private readonly IAppointmentrequestRepository _appointmentRequestRepository;
        private readonly IMapper _mapper;

        public AppointmentrequestService(IAppointmentrequestRepository appointmentRequestRepository, IMapper mapper)
        {
            _appointmentRequestRepository = appointmentRequestRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentrequestModel> CreateAppointmentRequestAsync(AppointmentrequestModel appointmentRequestModel)
        {
            try
            {
                var appointmentRequest = _mapper.Map<Appointmentrequest>(appointmentRequestModel);
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

        public async Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserIdAsync(parameters, userId);
                var appointmentRequestModels = _mapper.Map<PagedList<AppointmentrequestModel>>(result);

                return new PagedList<AppointmentrequestModel>(appointmentRequestModels, appointmentRequestModels.Count, appointmentRequestModels.CurrentPage, appointmentRequestModels.PageSize);
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

                return new PagedList<AppointmentrequestModel>(appointmentRequestModels, appointmentRequestModels.Count, appointmentRequestModels.CurrentPage, appointmentRequestModels.PageSize);
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
    }
}
