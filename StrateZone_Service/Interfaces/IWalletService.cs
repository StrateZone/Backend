using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IWalletService
    {
        Task<WalletModel> CreateWalletAsync(WalletModel wallet);
        Task DepositWalletAsync(int amount, int id);
        Task DepositWalletByUserIdAsync(int amount, int userId);
        Task<WalletModel> GetWalletByIdAsync(int id);
        Task<WalletModel> GetWalletByUserIdAsync(int userId);
        Task<WalletModel> UpdateWalletAsync(WalletModel wallet, int id);
        Task<WalletModel> WithdrawalWalletAsync(int amount, int id);
        Task<WalletModel> WithdrawalWalletByUserIdAsync(int amount, int userId);
        Task WithdrawalBalanceToOtherUserAsync(int amount, string msg, int fromUser, int toUser);
    }
}
