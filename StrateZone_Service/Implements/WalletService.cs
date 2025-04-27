using AutoMapper;
using CloudinaryDotNet;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
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
