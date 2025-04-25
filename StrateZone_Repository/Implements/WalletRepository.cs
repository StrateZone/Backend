using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System.Text;

namespace StrateZone_Repository.Implements
{
    public class WalletRepository : IWalletRepository
    {
        private readonly StrateZoneDbContext _context;

        public WalletRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateWalletAsync(Wallet wallet)
        {
            try
            {
                if (await _context.Wallets.AnyAsync(w => w.UserId == wallet.UserId))
                    throw new Exception("Wallet for this user ID already exists.");

                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO wallet (user_id, balance, status) 
                    VALUES (@user_id, @balance, @status::wallet_status)
                    RETURNING wallet_id;";

                cmd.Parameters.Add(new NpgsqlParameter("@user_id", wallet.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("@balance", wallet.Balance));
                cmd.Parameters.Add(new NpgsqlParameter("@status", wallet.Status.ToString()));

                var newWalletId = await cmd.ExecuteScalarAsync();
                wallet.WalletId = Convert.ToInt32(newWalletId);

                return wallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Wallet> GetWalletByIdAsync(int id)
        {
            try
            {
                return await _context.Wallets.FindAsync(id) ?? throw new Exception("No wallet with this ID was found.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Wallet> GetWalletByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Wallets.AsNoTracking().Include(u => u.User).SingleOrDefaultAsync(w => w.UserId == userId) ?? throw new Exception("No wallet for this user was found.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Wallet> UpdateWalletAsync(Wallet wallet, int id)
        {
            try
            {
                var existingWallet = await _context.Wallets.FindAsync(id)
                        ?? throw new KeyNotFoundException("Wallet with this ID does not exist.");

                _context.Entry(existingWallet).State = EntityState.Detached;

                wallet.WalletId = id;
                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE wallet SET ");

                if (wallet.Balance.HasValue)
                {
                    sql.Append("balance = @balance, ");
                    parameters.Add(new NpgsqlParameter("@balance", wallet.Balance.Value));
                }

                sql.Append("status = @status::wallet_status, ");
                parameters.Add(new NpgsqlParameter("@status", wallet.Status.ToString()));

                if (wallet.UserId.HasValue)
                {
                    sql.Append("user_id = @user_id, ");
                    parameters.Add(new NpgsqlParameter("@user_id", wallet.UserId.Value));
                }

                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE wallet_id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var updatedWallet = await _context.Wallets.FindAsync(id);
                return updatedWallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Wallet> DepositWalletAsync(int amount, int id)
        {
            try
            {
                if (amount <= 0) throw new Exception("Deposit amount must be higher than 0.");

                Wallet wallet = await _context.Wallets.FindAsync(id)
                    ?? throw new KeyNotFoundException("Wallet with this ID does not exist.");

                if (wallet.Status == Parameters.PostgreEnums.WalletStatus.closed)
                    throw new Exception("This wallet is closed.");

                wallet.Balance += amount;
                await _context.SaveChangesAsync();

                return wallet;

            }
            catch
            {
                throw;
            }
        }

        public async Task<Wallet> WithdrawalWalletAsync(int amount, int id)
        {
            try
            {
                if (amount <= 0) throw new Exception("Withdrawal amount must be higher than 0.");

                Wallet wallet = await _context.Wallets.FindAsync(id) ?? throw new Exception("Wallet with this ID does not exist.");

                if (wallet.Status == Parameters.PostgreEnums.WalletStatus.closed)
                    throw new Exception("This wallet is closed.");
                else if (wallet.Balance < amount)
                    throw new Exception("Wallet's balance is lower than the withdrawal amount.");

                wallet.WalletId = id;
                wallet.Balance -= amount;

                await _context.SaveChangesAsync();

                return wallet;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
