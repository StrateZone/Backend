using AutoMapper;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Microsoft.Extensions.DependencyInjection;

namespace StrateZone_Service.Implements
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUserService _userService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public VoucherService(IVoucherRepository voucherRepository, IMapper mapper, IUserService userService, IServiceScopeFactory serviceScopeFactory)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
            _userService = userService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<VoucherModel> CreateSampleVoucherAsync(SampleVoucherRequest request)
        {
            try
            {
                VoucherModel voucherModel = new()
                {
                    VoucherName = request.VoucherName,
                    Value = request.Value,
                    Description = request.Description,
                    MinPriceCondition = request.MinPriceCondition,
                    IsSample = true,
                    UserId = null,
                    PointsCost = request.PointsCost,
                    ContributionPointsCost = request.ContributorPointsCost,
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

        public async Task<VoucherModel> CreateVoucherFromSampleAsync(UserVoucherRequest voucher)
        {
            try
            {
                var sample = await GetByIdAsync(voucher.SampleVoucherId) ?? throw new Exception("No voucher with this ID was found");
                if (!sample.IsSample) throw new Exception("This is not a sample voucher.");

                var user = await _userService.GetUserByIdAsync(voucher.UserId)
                    ?? throw new Exception("No user with this ID was found");

                var pointsCost = user.UserLabel == UserLabel.top_contributor.ToString() ? sample.ContributionPointsCost : sample.PointsCost;

                if (user.Points < pointsCost)
                    throw new Exception("You don't have enough points to exchange this voucher.");

                user.Points -= pointsCost;
                await _userService.UpdateUserAsync(_mapper.Map<UserModel>(user), user.UserId);

                VoucherModel voucherModel = new()
                {
                    VoucherName = sample.VoucherName,
                    Value = sample.Value,
                    Description = sample.Description,
                    MinPriceCondition = sample.MinPriceCondition,
                    UserId = user.UserId,
                    IsSample = false,
                    PointsCost = 0,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                };

                Voucher createdVoucher = _mapper.Map<Voucher>(voucherModel);
                var result = await _voucherRepository.CreateVoucherAsync(createdVoucher);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var notiService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var pointService = scope.ServiceProvider.GetRequiredService<IPointsHistoryService>();

                    PointsHistoryModel pointHistoryModel = new()
                    {
                        OfUser = user.UserId,
                        Content = $"-{pointsCost} điểm cá nhân: Đổi voucher {result.VoucherName}",
                        Amount = pointsCost,
                        PointType = "personal_point",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    };
                    await pointService.AddAsync(pointHistoryModel);

                    NotificationRequest notification = new()
                    {
                        ToUser = user.UserId,
                        Title = "Đổi voucher thành công!",
                        Content = $"Bạn đã đổi thành công voucher {result.VoucherName}.",
                        Type = PostgreEnums.NotificationType.points_history,
                    };
                    await notiService.CreateNotificationAsync(notification);
                });

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

        public async Task<PagedList<VoucherModel>> GetVouchersByUserIdAsync(TablesAppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _voucherRepository.GetVouchersByUserIdAsync(parameters, userId);

                var mapped = _mapper.Map<PagedList<VoucherModel>>(result);

                return new PagedList<VoucherModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<VoucherModel>> GetSampleVouchersAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _voucherRepository.GetSampleVouchersAsync(parameters);

                var mapped = _mapper.Map<PagedList<VoucherModel>>(result);

                return new PagedList<VoucherModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
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

        public async Task<List<VoucherModel>> UpdateVouchersAsync(List<VoucherModel> voucherModel)
        {
            try
            {
                var vouchers = _mapper.Map<List<Voucher>>(voucherModel);
                var result = await _voucherRepository.UpdateVouchersAsync(vouchers);

                return _mapper.Map<List<VoucherModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<VoucherModel>> UseVouchersAsync(List<int> voucherIds, int userId)
        {
            try
            {
                var vouchers = await _voucherRepository.GetVoucherByIdsAsync([.. voucherIds]);

                if (vouchers.Any(v => v.UserId != userId))
                    throw new Exception("One or more vouchers used do not belong to this user!");

                if (vouchers.Any(v => v.Status == PostgreEnums.VoucherStatus.expired))
                    throw new Exception("One or more vouchers have been expired!");

                vouchers.ForEach(v => v.Status = PostgreEnums.VoucherStatus.expired);
                
                var result = await _voucherRepository.UpdateVouchersAsync(_mapper.Map<List<Voucher>>(vouchers));

                return _mapper.Map<List<VoucherModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
