using System.Data;
using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Repository;
using TestApplication.Data;
using UP.Controllers;

namespace UP.Repositories
{
    public class TransactionsRepository: RepositoryBase, ITransactionsRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<TransactionController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public TransactionsRepository(DataContext context, ILogger<TransactionController> logger, IUserRepository userRepository, ICurrencyRepository currencyRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
            _currencyRepository = currencyRepository;
        }

        public void WriteNewConversionDataToDatabase(ModelsEF.Conversion conversion)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO conversions (commission, begin_coin_quantity, end_coin_quantity, quantity_usd, begin_coin_shortname, end_coin_shortname, user_id, date) " +
                      "VALUES (@commission, @begin_coin_quantity, @end_coin_quantity, @quantity_usd, @begin_coin_shortname, @end_coin_shortname, @user_id, @date)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@commission", conversion.Commission);
            command.Parameters.AddWithValue("@begin_coin_quantity", conversion.BeginCoinQuantity);
            command.Parameters.AddWithValue("@end_coin_quantity", conversion.EndCoinQuantity);
            command.Parameters.AddWithValue("@quantity_usd", conversion.QuantityUSD);
            command.Parameters.AddWithValue("@begin_coin_shortname", conversion.BeginCoinShortname);
            command.Parameters.AddWithValue("@end_coin_shortname", conversion.EndCoinShortname);
            command.Parameters.AddWithValue("@user_id", conversion.UserId);
            command.Parameters.AddWithValue("@date", conversion.DateCreated);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
        
        public List<ModelsEF.Conversion> GetUserConversionsHistory(Guid userId)
        {
            var user = _context.Users
                .Include(u => u.Conversions).Include(user => user.Conversions)
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.Conversions.ToList();
            }
            else
            {
                return new List<ModelsEF.Conversion>();
            }
        }

        public async void ReplenishTheBalance(Guid userId, double quantityUsd)
        {
            double commission = 0.02;
            _currencyRepository.AddCryptoToUserWallet(userId, "usdt", quantityUsd - quantityUsd * commission);
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO replenishment (date, quantity, commission, user_id) VALUES (@date, @quantity, @commission, @user_id)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@quantity", quantityUsd - quantityUsd * commission);
            command.Parameters.AddWithValue("@commission", commission * quantityUsd);
            command.Parameters.AddWithValue("@user_id", userId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
        
        public List<ModelsEF.Replenishment> GetUserDepositHistory(Guid userId)
        {
            var user = _context.Users
                .Include(u => u.Replenishments)
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.Replenishments.ToList();
            }
            else
            {
                return new List<ModelsEF.Replenishment>();
            }
        }
        
        public async void WithdrawUSDT(Guid userId, double quantityUsd)
        {
            double commission = 0.02;
            _currencyRepository.SellCrypto(userId, "usdt", quantityUsd + quantityUsd * commission);
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO withdrawals (date, quantity, commission, user_id) VALUES (@date, @quantity, @commission, @user_id)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@quantity", quantityUsd);
            command.Parameters.AddWithValue("@commission", commission * quantityUsd);
            command.Parameters.AddWithValue("@user_id", userId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
        
        public List<ModelsEF.Withdrawal> GetUserWithdrawalsHistory(Guid userId)
        {
            var user = _context.Users
                .Include(u => u.Replenishments).Include(user => user.Withdrawals)
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.Withdrawals.ToList();
            }
            else
            {
                return new List<ModelsEF.Withdrawal>();
            }
        }
        
        public List<ModelsEF.Transactions> GetUserTransactionsHistory(Guid userId)
        {
            var user = _context.Users
                .Include(u => u.SentTransactions).Include(user => user.Withdrawals)
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.SentTransactions.ToList();
            }
            else
            {
                return new List<ModelsEF.Transactions>();
            }
        }
    }
}