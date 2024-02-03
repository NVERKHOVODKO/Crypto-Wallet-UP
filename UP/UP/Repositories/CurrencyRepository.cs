using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Npgsql;
using Repository;
using TestApplication.Data;
using UP.Models;
using UP.Models.Base;

namespace UP.Repositories
{
    public class CurrencyRepository: RepositoryBase, ICurrencyRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context;

        public CurrencyRepository(DataContext context)
        {
            _context = context;
        }
        public void BuyCrypto(Guid userId, string shortname, double quantity)
        {
            AddCryptoToUserWallet(userId, shortname, quantity);
        }
        
        public List<ModelsEF.Coin> GetUserCoins(Guid userId)
        {
            var userCoins = _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UsersCoins)
                .Select(uc => uc.Coin)
                .ToList();

            return userCoins;
        }

        public void AddCryptoToUserWallet(Guid userId, string shortname, double quantity)
        {
            List<ModelsEF.Coin> coins = GetUserCoins(userId);
            
            if (IsCoinAlreadyPurchased(coins, shortname)) {
                Guid coinId = GetPurchasedCoinId(coins, shortname);
                int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
                if (coinId != Guid.Empty)
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
        
        public void SendCrypto(Guid receiverId, Guid senderId, string shortname, double quantity)
        {
            SubtractCoinFromUser(senderId, shortname, quantity);
            AddCryptoToUserWallet(receiverId, shortname, quantity);
        }

        public bool IsCoinAlreadyPurchased(List<ModelsEF.Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.Shortname == shortName)
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
        
        private Guid GetPurchasedCoinId(List<ModelsEF.Coin> coins, string shortName)
        {
            try
            {
                foreach (var i in coins)
                {
                    if (i.Shortname == shortName)
                    {
                        return i.Id;
                    }
                }
                return Guid.Empty;
            }
            catch (Exception e)
            {
                return Guid.Empty;
            }
        }
        
        private int GetPurchasedCoinNumberInTheList(List<ModelsEF.Coin> coins, string shortName)
        {
            try
            {
                int j = 0;
                foreach (var i in coins)
                {
                    if (i.Shortname == shortName)
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
        
        public async void SellCrypto(Guid userId, string shortname, double quantityForSale)
        {
            double quantityInUserWallet = _userRepository.GetCoinQuantityInUserWallet(userId, "usdt");
            if (await GetCoinPrice(quantityInUserWallet, "usdt") < await GetCoinPrice(quantityForSale, shortname))
            {
                return;
            }
            SubtractCoinFromUser(userId, shortname, quantityForSale);
        }
        
        public void SubtractCoinFromUser(Guid userId, string shortname, double quantityForSubtract)
        {
            Console.WriteLine("Id: " + userId + " Name: " + shortname + " quantityForSubtract: " + quantityForSubtract);
            List<ModelsEF.Coin> coins = _userRepository.GetUserCoins(userId);
            Guid coinId = GetPurchasedCoinId(coins, shortname);
            int coinIdInTheList = GetPurchasedCoinNumberInTheList(coins, shortname);
            if (coinId != Guid.Empty)
            {
                Console.WriteLine("Id: " + coins[coinIdInTheList].Id + " Quantity: " + coins[coinIdInTheList].Quantity + " ShortName: " + coins[coinIdInTheList].Shortname);
                var coin = new ModelsEF.Coin(coins[coinIdInTheList].Id, coins[coinIdInTheList].Quantity, coins[coinIdInTheList].Shortname);
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
        
        public async Task<double> GetUserBalance(Guid userId)
        {
            var coins = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UsersCoins)
                .ThenInclude(uc => uc.Coin)
                .SelectMany(u => u.UsersCoins.Select(uc => uc.Coin))
                .ToList();
            double balance = 0;
            foreach (var i in coins)
            {
                balance += await GetCoinPrice(i.Quantity, i.Shortname);
            }
            return balance;
        }
        
        public void DeleteCoin(Guid coinId)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "DELETE FROM coins WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", coinId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }

        /*//3 sec
        public async Task<double> GetDailyPriceImpact(string shortName)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            string url = $"https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + shortName + "&tsyms=USD";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(responseContent);
            var priceData = data["RAW"]["BTC"]["USD"];
            var priceChange = (double)priceData["CHANGEDAY"];
            return priceChange;
        }*/

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
        private const string CryptoCompareApiUrl = "https://min-api.cryptocompare.com";

        public async Task<CoinsInformation> GetFullCoinInformation(string shortName)
        {
            Dictionary<string, string> cryptoDictionary = CoinList.GetCryptoDictionary();
            string fullName = cryptoDictionary[shortName];

            string url = $"{CryptoCompareApiUrl}/data/pricemultifull?fsyms={shortName}&tsyms=USD";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-MBX-APIKEY", apiKey);
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

        public void UpdateCoinQuantity(Guid id, double quantity)
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
        
        public void WriteTransactionToDatabase(string coinName, double quantity, Guid senderId, Guid receiverId)
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
        
        public void WriteWithdrawToDatabase(double quantity, double commission, Guid userId)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "INSERT INTO withdrawals (date, quantity, commission, user_id) VALUES " +
                      "(@date, @quantity, @commission, @user_id)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@commission", commission);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@user_id", userId);
            OpenConnection(connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }
    }
}