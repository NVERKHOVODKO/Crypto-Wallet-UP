using System.Data;
using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;

namespace UP.Repositories
{
    public class TransactionsRepository: RepositoryBase
    {
        public void WriteNewConversionDataToDatabase(Conversion conversion)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO conversions (commission, begin_coin_quantity, end_coin_quantity, quantity_usd, begin_coin_shortname, end_coin_shortname, user_id, date) " +
                      "VALUES (@commission, @begin_coin_quantity, @end_coin_quantity, @quantity_usd, @begin_coin_shortname, @end_coin_shortname, @user_id, @date)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@commission", conversion.Commission);
            command.Parameters.AddWithValue("@begin_coin_quantity", conversion.BeginCoinQuantity);
            command.Parameters.AddWithValue("@end_coin_quantity", conversion.EndCoinQuantity);
            command.Parameters.AddWithValue("@quantity_usd", conversion.QuantityUsd);
            command.Parameters.AddWithValue("@begin_coin_shortname", conversion.BeginCoinShortname);
            command.Parameters.AddWithValue("@end_coin_shortname", conversion.EndCoinShortname);
            command.Parameters.AddWithValue("@user_id", conversion.UserId);
            command.Parameters.AddWithValue("@date", conversion.Date);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
        
        public List<Models.Conversion> GetUserConversionsHistory(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * " +
                               "FROM conversions c " +
                               "WHERE c.user_id = @userId";
                var conversions = new List<Conversion>();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            double commission = reader.GetDouble(1);
                            double beginCoinQuantity = reader.GetDouble(2);
                            double endCoinQuantity = reader.GetDouble(3);
                            double quantityUsd = reader.GetDouble(4);
                            string beginCoinShortName = reader.GetString(5);
                            string endCoinShortName = reader.GetString(6);
                            int userIdRead = reader.GetInt32(7);
                            DateTime dateTime = reader.GetDateTime(8);
                            
                            conversions.Add(new Conversion(id, commission, beginCoinQuantity, endCoinQuantity, quantityUsd, beginCoinShortName, endCoinShortName,userIdRead, dateTime));
                        }
                    }
                }
                connection.Close();
                return conversions;
            }
        }

        public async void ReplenishTheBalance(int userId, double quantityUsd)
        {
            var cr = new Repositories.CurrencyRepository();
            double commission = 0.02;
            cr.AddCryptoToUserWallet(userId, "usdt", quantityUsd - quantityUsd * commission);
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
        
        public List<Models.Replenishment> GetUserDepositHistory(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * " +
                               "FROM replenishment c " +
                               "WHERE c.user_id = @userId";
                var replenishment = new List<Replenishment>();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            DateTime dateTime = reader.GetDateTime(1);
                            double quantity = reader.GetDouble(2);
                            double commission = reader.GetDouble(3);
                            int userIdRead = reader.GetInt32(4);
                            replenishment.Add(new Replenishment(id, dateTime, quantity, commission, userIdRead));
                        }
                    }
                }
                connection.Close();
                return replenishment;
            }
        }
        
        public async void WithdrawUSDT(int userId, double quantityUsd)
        {
            var cr = new Repositories.CurrencyRepository();
            double commission = 0.02;
            cr.SellCrypto(userId, "usdt", quantityUsd + quantityUsd * commission);
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
        
        public List<Models.Withdrawal> GetUserWithdrawalsHistory(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * " +
                               "FROM withdrawals c " +
                               "WHERE c.user_id = @userId";
                var withdrawals = new List<Withdrawal>();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            DateTime dateTime = reader.GetDateTime(1);
                            double quantity = reader.GetDouble(2);
                            double commission = reader.GetDouble(3);
                            int userIdRead = reader.GetInt32(4);
                            withdrawals.Add(new Withdrawal(id, dateTime, quantity, commission, userIdRead));
                        }
                    }
                }
                connection.Close();
                return withdrawals;
            }
        }
        
        public List<Models.Transactions> GetUserTransactionsHistory(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * " +
                               "FROM transactions c " +
                               "WHERE sender_id = @userId OR receiver_id = @userId";
                var transactions = new List<Transactions>();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string coinName = reader.GetString(1);
                            double quantity = reader.GetDouble(2);
                            DateTime dateTime = reader.GetDateTime(3);
                            int senderId = reader.GetInt32(4);
                            int receiverId = reader.GetInt32(5);
                            transactions.Add(new Transactions(id, coinName, quantity, dateTime, receiverId, senderId));
                        }
                    }
                }
                connection.Close();
                return transactions;
            }
        }
    }
}