using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> CreateWalletAsync(Wallet wallet);
        Task<Wallet> DepositWalletAsync(int amount, int id);
        Task<Wallet> GetWalletByIdAsync(int id);
        Task<Wallet> GetWalletByUserIdAsync(int userId);
        Task<Wallet> UpdateWalletAsync(Wallet wallet, int id);
        Task<Wallet> WithdrawalWalletAsync(int amount, int id);
    }
}