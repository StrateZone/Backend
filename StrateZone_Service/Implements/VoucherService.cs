using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherService(IVoucherRepository voucherRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<VoucherModel> CreateVoucherAsync(VoucherRequest request)
        {
            try
            {
                VoucherModel voucherModel = new()
                {
                    VoucherName = request.VoucherName,
                    Value = request.Value,
                    Description = request.Description,
                    MinPriceCondition = request.MinPriceCondition,
                    ExpireDate = request.ExpireDate,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                };

                Voucher voucher = _mapper.Map<Voucher>(voucherModel);
                var result = await _voucherRepository.CreateVoucherAsync(voucher);

                return _mapper.Map<VoucherModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VoucherModel> DeleteAsync(int id)
        {
            try
            {
                var result = await _voucherRepository.DeleteAsync(id);

                return _mapper.Map<VoucherModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VoucherModel> GetByIdAsync(int id)
        {
            try
            {
                var result = await _voucherRepository.GetByIdAsync(id);

                return _mapper.Map<VoucherModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VoucherModel> GetVoucherByPaymentid(int paymentId)
        {
            try
            {
                var result = await _voucherRepository.GetVoucherByPaymentid(paymentId);

                return _mapper.Map<VoucherModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<VoucherModel>> GetVouchersAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _voucherRepository.GetVouchersAsync(parameters);

                var mapped = _mapper.Map<PagedList<VoucherModel>>(result);
            
                return new PagedList<VoucherModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VoucherModel> UpdateVoucherAsync(VoucherModel voucherModel, int id)
        {
            try
            {
                var voucher = _mapper.Map<Voucher>(voucherModel);
                var result = await _voucherRepository.UpdateVoucherAsync(voucher, id);

                return _mapper.Map<VoucherModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
