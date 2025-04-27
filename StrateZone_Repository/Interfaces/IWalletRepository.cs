using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> CreateWalletAsync(Wallet wallet);
        Task DepositWalletAsync(int amount, int id);
        Task DepositWalletByUserIdAsync(int amount, int userId);
        Task<Wallet> GetWalletByIdAsync(int id);
        Task<Wallet> GetWalletByUserIdAsync(int userId);
        Task<Wallet> UpdateWalletAsync(Wallet wallet, int id);
        Task<Wallet> WithdrawalWalletAsync(int amount, int id);
    }
}
