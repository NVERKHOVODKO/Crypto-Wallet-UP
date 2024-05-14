using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ProjectX.Exceptions;
using Repository;
using TestApplication.Data;
using UP.Models;
using UP.Models.Base;
using UP.ModelsEF;
using Coin = UP.ModelsEF.Coin;
using Exception = System.Exception;
using Transactions = UP.ModelsEF.Transactions;
using Withdrawal = UP.ModelsEF.Withdrawal;

namespace UP.Repositories;

public class CurrencyRepository : RepositoryBase, ICurrencyRepository
{
    private const string CryptoCompareApiUrl = "https://min-api.cryptocompare.com";
    private static readonly HttpClient httpClient = new();
    private readonly DataContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IDbRepository _dbRepository;

    public CurrencyRepository(DataContext context, IDbRepository dbRepository)
    {
        _context = context;
        _dbRepository = dbRepository;
    }

    public void BuyCrypto(Guid userId, string shortname, double quantity)
    {
        AddCryptoToUserWallet(userId, shortname, quantity);
    }

    public List<Coin> GetUserCoins(Guid userId)
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
        var user = _context.Users.FirstOrDefault(uc => uc.Id == userId);
        
        if(user?.Login == "Service" || user == null)
            return;
        
        var userCoins = _context.UsersCoins
            .Include(uc => uc.Coin)
            .Where(uc => uc.UserId == userId)
            .ToList();

        var existingCoin = userCoins.FirstOrDefault(uc => uc.Coin.Shortname == shortname);

        if (existingCoin != null)
        {
            existingCoin.Coin.Quantity += quantity;
        }
        else
        {
            var newCoin = new Coin
            {
                Quantity = quantity,
                Shortname = shortname
            };

            _context.Coins.Add(newCoin);

            var userCoin = new UsersCoins
            {
                UserId = userId,
                Coin = newCoin
            };

            _context.UsersCoins.Add(userCoin);
        }
        _context.SaveChanges();
    }

    public void SendCrypto(Guid receiverId, Guid senderId, string shortname, double quantity)
    {
        SubtractCoinFromUser(senderId, shortname, quantity);
        AddCryptoToUserWallet(receiverId, shortname, quantity);
    }

    public bool IsCoinAlreadyPurchased(List<Coin> coins, string shortName)
    {
        try
        {
            return coins.Any(i => i.Shortname == shortName);
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async void SellCrypto(Guid userId, string shortname, double quantityForSale)
    {
        var quantityInUserWallet = _userRepository.GetCoinQuantityInUserWallet(userId, "usdt");
        if (await GetCoinPrice(quantityInUserWallet, "usdt") < await GetCoinPrice(quantityForSale, shortname))
            return;

        SubtractCoinFromUser(userId, shortname, quantityForSale);
    }

    public void SubtractCoinFromUser(Guid userId, string shortname, double quantityForSubtract)
    {
        var userCoins = _context.UsersCoins
            .Include(uc => uc.Coin)
            .Where(uc => uc.UserId == userId)
            .ToList();

        var existingCoin = userCoins.FirstOrDefault(uc => uc.Coin.Shortname == shortname);

        if (existingCoin?.Coin.Shortname != null)
        {
            existingCoin.Coin.Quantity -= quantityForSubtract;
            if (existingCoin.Coin.Quantity / quantityForSubtract < 1.05)
            {
                _context.Remove(existingCoin);
            }
        }
        
        _context.SaveChanges();
    }

    public async Task<double> GetCoinPrice(double quantity, string shortName)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", ApiKey);
        var url = "https://min-api.cryptocompare.com/data/price?fsym=" + shortName + "&tsyms=USD";
        var response = await httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
        var json = JObject.Parse(responseContent);
        var price = (double)json["USD"] * quantity;
        return price;
    }

    public async Task<double> GetCoinQuantity(double quantityUSD, string shortName)
    {
        var price = await GetCoinPrice(1, shortName);
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
        foreach (var i in coins) balance += await GetCoinPrice(i.Quantity, i.Shortname);
        return balance;
    }

    public void DeleteCoin(Guid coinId)
    {
        var coinToDelete = _context.Coins.Find(coinId);

        if (coinToDelete != null)
        {
            _context.Coins.Remove(coinToDelete);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Невозможно удалить монету");
        }
    }

    public async Task<CoinsInformation> GetFullCoinInformation(string shortName)
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .Where(x => x.IsActive)
            .ToListAsync();

        var cryptoDictionary = coinsList.ToDictionary(
            coin => coin.ShortName.ToLower(),
            coin => coin.FullName.ToLower()
        );
        var fullName = cryptoDictionary[shortName];

        var url = $"{CryptoCompareApiUrl}/data/pricemultifull?fsyms={shortName}&tsyms=USD";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-MBX-APIKEY", ApiKey);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to get coin information: {response.StatusCode}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JObject.Parse(responseContent);
        var priceData = data["RAW"][shortName.ToUpper()]["USD"];
        var priceChange = (double)priceData["CHANGEDAY"];
        var dailyVolume = (double)priceData["VOLUME24HOUR"];
        var number = dailyVolume;
        var price = (double)priceData["PRICE"];
        var previousPrice = price - priceChange;
        var percentagePriceChangePerDay = priceChange / previousPrice * 100;
        return new CoinsInformation(fullName, shortName,
            @"C:\НЕ СИСТЕМА\BSUIR\второй курс\UP\cryptoicons_png\128\" + shortName.ToLower(), dailyVolume, priceChange,
            price, percentagePriceChangePerDay);
    }

    public void UpdateCoinQuantity(Guid id, double quantity)
    {
        var coinToUpdate = _context.Coins.Find(id);

        if (coinToUpdate == null) return;
        coinToUpdate.Quantity = quantity;
        _context.SaveChanges();
    }

    public void WriteTransactionToDatabase(string coinName, double quantity, Guid senderId, Guid receiverId)
    {
        var transaction = new Transactions
        {
            Id = Guid.NewGuid(),
            CoinName = coinName,
            Quantity = quantity,
            SenderId = senderId,
            ReceiverId = receiverId,
            DateCreated = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void WriteWithdrawToDatabase(double quantity, double commission, Guid userId)
    {
        var withdrawal = new Withdrawal
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            Commission = commission,
            UserId = userId,
            DateCreated = DateTime.UtcNow
        };

        _context.Withdrawals.Add(withdrawal);
        _context.SaveChanges();
    }
}