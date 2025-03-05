using AutoMapper;
using Azure.Core;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public TableService(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        public async Task<List<TableModel>> GetTablesAsync()
        {
            try
            {
                var result = await _tableRepository.GetTablesAsync();
                return _mapper.Map<List<TableModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TableModel>> GetAvailableTablesAsync()
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesAsync();
                return _mapper.Map<List<TableModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TableModel>> GetTablesByGameTypeAsync(StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var result = await _tableRepository.GetTablesByGameTypeAsync(gameType);
                return _mapper.Map<List<TableModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TableModel>> GetAvailableTablesByGameTypeAsync(StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesByGameTypeAsync(gameType);
                return _mapper.Map<List<TableModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> GetTableByIdAsync(int id)
        {
            try
            {
                var result = await _tableRepository.GetTableByIdAsync(id);
                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> CreateTableAsync(TableRequest request)
        {
            try
            {
                TableModel model = new TableModel()
                {
                    RoomId = request.Room_Id,
                    GameTypeId = request.GameType_Id
                };

                var toAdd = _mapper.Map<Table>(model);
                var result = await _tableRepository.CreateTableAsync(toAdd);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> UpdateTableAsync(TableModel tableModel, int id)
        {
            try
            {
                var toAdd = _mapper.Map<Table>(tableModel);
                var result = await _tableRepository.UpdateTableAsync(toAdd, id);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> DeleteTableAsync(int id)
        {
            try
            {
                var result = await _tableRepository.DeleteTableAsync(id);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
