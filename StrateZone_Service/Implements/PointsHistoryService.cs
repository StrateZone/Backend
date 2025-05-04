using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Pagination;
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
    public class PointsHistoryService : IPointsHistoryService
    {
        private readonly IPointsHistoryRepository _repository;
        private readonly IMapper _mapper;

        public PointsHistoryService(IPointsHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PointsHistoryModel> AddAsync(PointsHistoryModel model)
        {
            var entity = _mapper.Map<PointsHistory>(model);
            var result = await _repository.AddAsync(entity);
            return _mapper.Map<PointsHistoryModel>(result);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<PagedList<PointsHistoryModel>> GetAllAsync(TablesAppointmentParameters parameters)
        {
            var result = await _repository.GetAllAsync(parameters);
            var mapped = _mapper.Map<PagedList<PointsHistoryModel>>(result);
            return new PagedList<PointsHistoryModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        public async Task<PointsHistoryModel> GetByIdAsync(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<PointsHistoryModel>(result);
        }

        public async Task<PagedList<PointsHistoryModel>> GetByUserIdAsync(int userId, TablesAppointmentParameters parameters)
        {
            var result = await _repository.GetByUserIdAsync(userId, parameters);
            var mapped = _mapper.Map<PagedList<PointsHistoryModel>>(result);
            return new PagedList<PointsHistoryModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        public async Task UpdateAsync(PointsHistoryModel model, int id)
        {
            var entity = _mapper.Map<PointsHistory>(model);
            entity.Id = id;
            await _repository.UpdateAsync(entity, id);
        }
    }
}
