using Newtonsoft.Json.Linq;
using Npgsql;
using UP.Models;
using UP.Models.Base;

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
            var ur = new UserRepository();
            List<Coin> coins = ur.GetUserCoins(userId);
            
            if (IsCoinAlreadyPurchased(coins, shortname)) {
                int coinId = GetPurchasedCoinId(coins, shortname);
                int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
                if (coinId != -1)
                {
                    double finalQuantity = coins[coinIdInTheList].Quantity + quantity; 
                    UpdateCoinQuantity(coins[coinIdInTheList].Id, finalQuantity);
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
        
        public void SendCrypto(int receiverId, int senderId, string shortname, double quantity)
        {
            SubtractCoinFromUser(senderId, shortname, quantity);
            AddCryptoToUserWallet(receiverId, shortname, quantity);
        }

        public bool IsCoinAlreadyPurchased(List<Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.ShortName == shortName)
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
        
        private int GetPurchasedCoinId(List<Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.ShortName == shortName)
                    {
                        return i.Id;
                    }
                }
                return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        
        private int GetPurchasedCoinNumberInTheList(List<Coin> coins, string shortName)
        {
            try
            {
                int j = 0;
                foreach (var i in coins)
                {
                    if (i.ShortName == shortName)
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
            var ur = new UserRepository();
            var cr = new CurrencyRepository();
            double quantityInUserWallet = ur.GetCoinQuantityInUserWallet(userId, "usdt");
            if (await cr.GetCoinPrice(quantityInUserWallet, "usdt") < await cr.GetCoinPrice(quantityForSale, shortname))
            {
                return;
            }
            SubtractCoinFromUser(userId, shortname, quantityForSale);
        }
        
        public void SubtractCoinFromUser(int userId, string shortname, double quantityForSubtract)
        {
            var ur = new UserRepository();
            List<Coin> coins = ur.GetUserCoins(userId);
            int coinId = GetPurchasedCoinId(coins, shortname);
            int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
            if (coinId != -1)
            {
                var coin = new Coin(coins[coinIdInTheList].Id, coins[coinIdInTheList].Quantity, coins[coinIdInTheList].ShortName);
                double finalQuantity = coin.Quantity - quantityForSubtract;
                if (finalQuantity == 0) {
                    DeleteCoin(coin.Id);
                }else if(finalQuantity > 0) {
                    UpdateCoinQuantity(coin.Id, finalQuantity);
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
        
        public async Task<double> GetCoinQuantity(double quantityUSD, string shortName)
        {
            double price = await GetCoinPrice(1 , shortName);
            return quantityUSD / price;
        }
        
        public async Task<double> GetUserBalance(int userId)
        {
            var ur = new UserRepository();
            List<Coin> coins = ur.GetUserCoins(userId);
            double balance = 0;
            foreach (var i in coins)
            {
                balance += await GetCoinPrice(i.Quantity, i.ShortName);
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

        //3 sec
        public async Task<double> GetDailyPriceImpact(string shortName)
        {
            string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            string url = $"https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + shortName + "&tsyms=USD";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(responseContent);
            var priceData = data["RAW"]["BTC"]["USD"];
            var priceChange = (double)priceData["CHANGEDAY"];
            return priceChange;
        }

        /*public async Task<CoinsInformation> GetFullCoinInformation(string shortName) 10 sec
        {
            var coin = new CoinsInformation();
            Dictionary<string, string> cryptoDictionary = CoinList.GetCryptoDictionary();
            string fullName = cryptoDictionary[shortName];
            string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            string url = $"https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + shortName + "&tsyms=USD";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();//server response
            var data = JObject.Parse(responseContent);
            var priceData = data["RAW"][shortName.ToUpper()]["USD"];
            var priceChange = (double)priceData["CHANGEDAY"];
            var dailyVolume = (double)priceData["VOLUME24HOUR"];
            var price = (double)priceData["PRICE"];
            
            return new CoinsInformation(fullName, shortName, @"C:\НЕ СИСТЕМА\BSUIR\второй курс\UP\cryptoicons_png\128\" + shortName.ToUpper(), dailyVolume, priceChange, price);
        }*/
        
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
        private const string CryptoCompareApiUrl = "https://min-api.cryptocompare.com";

        public async Task<CoinsInformation> GetFullCoinInformation(string shortName)
        {
            Dictionary<string, string> cryptoDictionary = CoinList.GetCryptoDictionary();
            string fullName = cryptoDictionary[shortName];

            string url = $"{CryptoCompareApiUrl}/data/pricemultifull?fsyms={shortName}&tsyms=USD";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-MBX-APIKEY", ApiKey);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get coin information: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(responseContent);
            var priceData = data["RAW"][shortName.ToUpper()]["USD"];
            var priceChange = (double)priceData["CHANGEDAY"];
            var dailyVolume = (double)priceData["VOLUME24HOUR"];
            double number = dailyVolume;
            var price = (double)priceData["PRICE"];
            var previousPrice = price - priceChange;
            var percentagePriceChangePerDay = (priceChange / previousPrice) * 100;
            return new CoinsInformation(fullName, shortName, @"C:\НЕ СИСТЕМА\BSUIR\второй курс\UP\cryptoicons_png\128\" + shortName.ToLower(), dailyVolume, priceChange, price, percentagePriceChangePerDay);
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
        
        public void WriteTransactionToDatabase(string coinName, double quantity, int senderId, int receiverId)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO transactions (coin_name, quantity, date, sender_id, receiver_id) VALUES " +
                      "(@coin_name, @quantity, @date, @sender_id, @receiver_id)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@coin_name", coinName);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@sender_id", senderId);
            command.Parameters.AddWithValue("@receiver_id", receiverId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
    }
}