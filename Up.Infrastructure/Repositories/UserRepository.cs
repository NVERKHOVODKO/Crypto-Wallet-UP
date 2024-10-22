namespace Up.Infrastructure.Repositories;

public class UserRepository(AppDbContext context, ICurrencyRepository currencyRepository)
    : RepositoryBase, IUserRepository
{
    public List<Coin> GetUserCoins(Guid userId)
    {
        var userCoins = context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.UsersCoins)
            .ThenInclude(uc => uc.Coin)
            .SelectMany(u => u.UsersCoins.Select(uc => uc.Coin))
            .ToList();

        return userCoins;
    }
    
    public async Task<List<CoinsInformation>> GetUserCoinsFull(Guid userId)
{
    var coins = await context.UsersCoins
        .Where(uc => uc.UserId == userId)
        .Include(uc => uc.Coin)
        .Select(uc => new CoinsInformation
        {
            Id = uc.Coin.Id,
            Quantity = uc.Coin.Quantity,
            ShortName = uc.Coin.Shortname
        })
        .ToListAsync();

    var  coinsFull = new List<CoinsInformation>();
    foreach (var coin in coins)
    {
        var temp = await currencyRepository.GetFullCoinInformation(coin.ShortName);
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
    }

    return coinsFull;
}


    public double GetCoinQuantityInUserWallet(Guid userId, string coinShortname)
    {
        var quantity = context.Coins
            .Join(
                context.UsersCoins,
                coin => coin.Id,
                userCoin => userCoin.CoinId,
                (coin, userCoin) => new { coin, userCoin })
            .Where(joined => joined.userCoin.UserId == userId && joined.coin.Shortname == coinShortname)
            .Select(joined => joined.coin.Quantity)
            .FirstOrDefault();

        return quantity;
    }
}