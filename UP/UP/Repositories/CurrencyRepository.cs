using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;

namespace UP.Repositories
{
    public class CurrencyRepository: RepositoryBase
    {
        public void BuyCrypto(int userId, string shortname, double quantity)
        {
            AddCryptoToUserWallet(userId, shortname, quantity);
        }

        public void AddCryptoToUserWallet(int userId, string shortname, double quantity)
        {
            var ur = new Repositories.UserRepository();
            List<Models.Coin> coins = ur.GetUserCoins(userId);
            
            if (IsCoinAlreadyPurchased(coins, shortname)) {
                int coinId = GetPurchasedCoinId(coins, shortname);
                int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
                if (coinId != -1)
                {
                    double finalQuantity = coins[coinIdInTheList].quantity + quantity; 
                    UpdateCoinQuantity(coins[coinIdInTheList].id, finalQuantity);
                }
            }
            else
            {
                using var connection = new NpgsqlConnection(connectionString);
                OpenConnection(connection);
                var sql = "INSERT INTO coins (shortname, quantity) " + //create coin
                          "VALUES (@shortname, @quantity)";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@shortname", shortname);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.ExecuteNonQuery();
                
                sql = "SELECT id FROM coins ORDER BY id DESC LIMIT 1;";//get coin id
                using var command1 = new NpgsqlCommand(sql, connection);
                int coinId = Convert.ToInt32(command1.ExecuteScalar());
                command1.ExecuteNonQuery();
                
                sql = "INSERT INTO l_users_coins (user_id, coin_id) " +//unite
                      "VALUES (@user_id, @coin_id)";
                using var command2 = new NpgsqlCommand(sql, connection);
                command2.Parameters.AddWithValue("@user_id", userId);
                command2.Parameters.AddWithValue("@coin_id", coinId);
                command2.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }

        public bool IsCoinAlreadyPurchased(List<Models.Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.shortName == shortName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        private int GetPurchasedCoinId(List<Models.Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.shortName == shortName)
                    {
                        return i.id;
                    }
                }
                return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        
        private int GetPurchasedCoinNumberInTheList(List<Models.Coin> coins, string shortName)
        {
            try
            {
                int j = 0;
                foreach (var i in coins)
                {
                    if (i.shortName == shortName)
                    {
                        return j;
                    }

                    j++;
                }
                return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        
        public async void SellCrypto(int userId, string shortname, double quantityForSale)
        {
            var ur = new Repositories.UserRepository();
            var cr = new Repositories.CurrencyRepository();
            double quantityInUserWallet = ur.GetCoinQuantityInUserWallet(userId, "usdt");
            if (await cr.GetCoinPrice(quantityInUserWallet, "usdt") < await cr.GetCoinPrice(quantityForSale, shortname))
            {
                return;
            }
            SubtractCoinFromUser(userId, shortname, quantityForSale);
        }
        
        public void SubtractCoinFromUser(int userId, string shortname, double quantityForSubtract)
        {
            var ur = new Repositories.UserRepository();
            List<Models.Coin> coins = ur.GetUserCoins(userId);
            int coinId = GetPurchasedCoinId(coins, shortname);
            int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
            if (coinId != -1)
            {
                var coin = new Models.Coin(coins[coinIdInTheList].id, coins[coinIdInTheList].quantity, coins[coinIdInTheList].shortName);
                double finalQuantity = coin.quantity - quantityForSubtract;
                if (finalQuantity == 0) {
                    DeleteCoin(coin.id);
                }else if(finalQuantity > 0) {
                    UpdateCoinQuantity(coin.id, finalQuantity);
                }
            }
        }
        
        public async Task<double> GetCoinPrice(double quantity, string shortName)
        {
            string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + shortName + "&tsyms=USD";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            JObject json = JObject.Parse(responseContent);
            double price = (double)json["USD"] * quantity;
            return price;
        }
        
        public async Task<double> GetUserBalance(int userId)
        {
            var ur = new Repositories.UserRepository();
            List<Models.Coin> coins = ur.GetUserCoins(userId);
            double balance = 0;
            foreach (var i in coins)
            {
                balance += await GetCoinPrice(i.quantity, i.shortName);
            }
            return balance;
        }
        
        public void DeleteCoin(int coinId)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "DELETE FROM coins WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", coinId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void UpdateCoinQuantity(int id, double quantity)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "UPDATE coins SET quantity = @quantity WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@quantity", quantity);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
    }
}