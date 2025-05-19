using AutoMapper;
using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
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
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public WalletService(IWalletRepository walletRepository, IMapper mapper, IUserRepository userService, IServiceScopeFactory serviceScopeFactory)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
            _userService = userService;
            _scopeFactory = serviceScopeFactory;
        }

        public async Task<WalletModel> CreateWalletAsync(WalletModel walletModel)
        {
            try
            {
                var wallet = _mapper.Map<Wallet>(walletModel);
                var result = await _walletRepository.CreateWalletAsync(wallet);

                return _mapper.Map<WalletModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DepositWalletAsync(int amount, int id)
        {
            try
            {
                await _walletRepository.DepositWalletAsync(amount, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DepositWalletByUserIdAsync(int amount, int userId)
        {
            try
            {
                await _walletRepository.DepositWalletByUserIdAsync(amount, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task WithdrawalBalanceToOtherUserAsync(int amount, string msg, int fromUser, int toUser)
        {
            try
            {
                var transferer = await _userService.GetUserByIdAsync(fromUser);
                var receiver = await _userService.GetUserByIdAsync(toUser);

                if (receiver.Status == UserStatus.Suspended)
                    throw new Exception("This user is curerntly being suspended");

                var walletFrom = await GetWalletByUserIdAsync(fromUser);
                var walletTo = await GetWalletByUserIdAsync(toUser);

                await WithdrawalWalletAsync(amount, walletFrom.WalletId);
                await DepositWalletAsync(amount, walletTo.WalletId);

                _ = Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                    var newTransaction = new StrateZone_Repository.Entities.Transaction
                    {
                        OfUser = transferer.UserId,
                        Amount = amount,
                        Content = $"Chuyển tiền đến {receiver.Username}: {amount}đ.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        TransactionType = PostgreEnums.TransactionType.withdrawal
                    };
                    await service.SaveTransaction(newTransaction);

                    var newTransaction2 = new StrateZone_Repository.Entities.Transaction
                    {
                        OfUser = receiver.UserId,
                        Amount = amount,
                        Content = $"Nhận tiền từ {transferer.Username}: {amount}đ.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        TransactionType = PostgreEnums.TransactionType.deposit
                    };

                    await service.SaveTransaction(newTransaction2);
                });

                _ = Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    NotificationRequest thisUser = new()
                    {
                        ToUser = receiver.UserId,
                        Title = $"{transferer.Username} đã chuyển tiền cho bạn.",
                        Content = $"{transferer.Username} đã chuyển cho bạn {amount}đ. Lời nhắn: {msg}."
                    };

                    NotificationRequest thatUser = new()
                    {
                        ToUser = transferer.UserId,
                        Title = $"Chuyển tiền cho {receiver.Username}.",
                        Content = $"Bạn đã chuyển {amount}đ đến {receiver.Username} kèm lời nhắn: {msg}."
                    };

                    await service.CreateNotificationsAsync(new() { thisUser, thatUser });
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WalletModel> GetWalletByIdAsync(int id)
        {
            try
            {
                var result = await _walletRepository.GetWalletByIdAsync(id);
                return _mapper.Map<WalletModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WalletModel> GetWalletByUserIdAsync(int userId)
        {
            try
            {
                var result = await _walletRepository.GetWalletByUserIdAsync(userId);
                return _mapper.Map<WalletModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WalletModel> UpdateWalletAsync(WalletModel walletModel, int id)
        {
            try
            {
                var wallet = _mapper.Map<Wallet>(walletModel);
                var result = await _walletRepository.UpdateWalletAsync(wallet, id);
                return _mapper.Map<WalletModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WalletModel> WithdrawalWalletAsync(int amount, int id)
        {
            try
            {
                var result = await _walletRepository.WithdrawalWalletAsync(amount, id);
                return _mapper.Map<WalletModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<WalletModel> WithdrawalWalletByUserIdAsync(int amount, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
