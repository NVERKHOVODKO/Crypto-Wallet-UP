using Microsoft.EntityFrameworkCore;
using Repository;
using TestApplication.Data;
using UP.Models.Base;
using UP.ModelsEF;

namespace UP.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    private readonly DataContext _context;
    private readonly ICurrencyRepository _currencyRepository;

    public UserRepository(DataContext context, ICurrencyRepository currencyRepository)
    {
        _context = context;
        _currencyRepository = currencyRepository;
    }

    public List<Coin> GetUserCoins(Guid userId)
    {
        var userCoins = _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.UsersCoins)
            .ThenInclude(uc => uc.Coin)
            .SelectMany(u => u.UsersCoins.Select(uc => uc.Coin))
            .ToList();

        return userCoins;
    }


    public async Task<List<CoinsInformation>> GetUserCoinsFull(Guid userId)
{
    var coins = await _context.UsersCoins
        .Where(uc => uc.UserId == userId)
        .Include(uc => uc.Coin)
        .Select(uc => new CoinsInformation
        {
            Id = uc.Coin.Id,
            Quantity = uc.Coin.Quantity,
            ShortName = uc.Coin.Shortname
        })
        .ToListAsync();

    List<CoinsInformation>  coinsFull = new List<CoinsInformation>();
    int i = 0;
    foreach (var coin in coins)
    {
        CoinsInformation temp = await _currencyRepository.GetFullCoinInformation(coin.ShortName);
        coinsFull.Add(new CoinsInformation
        {
            Id = temp.Id,
            ShortName = temp.FullName,
            FullName = temp.ShortName,
            IconPath = temp.IconPath,
            DailyVolume = temp.DailyVolume,
            DailyImpact = temp.DailyImpact,
            Price = temp.Price,
            PercentagePriceChangePerDay = temp.PercentagePriceChangePerDay,
            Quantity = GetCoinQuantityInUserWallet(userId, temp.FullName)
        });
        i++;
    }

    return coinsFull;
}


    public double GetCoinQuantityInUserWallet(Guid userId, string coinShortname)
    {
        var quantity = _context.Coins
            .Join(
                _context.UsersCoins,
                coin => coin.Id,
                userCoin => userCoin.CoinId,
                (coin, userCoin) => new { coin, userCoin })
            .Where(joined => joined.userCoin.UserId == userId && joined.coin.Shortname == coinShortname)
            .Select(joined => joined.coin.Quantity)
            .FirstOrDefault();

        return quantity;
    }
}