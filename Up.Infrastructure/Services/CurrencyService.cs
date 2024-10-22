using Up.Core.Exceptions;
using Up.Core.Models;
using Up.Infrastructure.Repositories.Interfaces;
using UP.Services.Interfaces;

namespace Up.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IDbRepository _dbRepository;
    
    public CurrencyService(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    
    public async void AddIfNotExists(string coinShortName, double price)
    {
        try
        {
            var coin = await _dbRepository.Get<CoinListInfo>()
                .FirstOrDefaultAsync(c => c.ShortName.ToLower() == coinShortName.ToLower());
            if (coin == null)
                throw new EntityNotFoundException($"Coin with short name {coinShortName} not found.");

            var currentTime = DateTime.UtcNow;
            var tenMinutesAgo = currentTime.AddMinutes(-10);

            var existingPrice = _dbRepository.Get<CryptoCurrencyPrices>()
                .FirstOrDefault(p => p.CoinId == coin.Id && p.Timestamp >= tenMinutesAgo);

            if (existingPrice != null) return;
            var newPrice = new CryptoCurrencyPrices
            {
                Id = Guid.NewGuid(),
                CoinShortName = coinShortName,
                Price = price,
                Timestamp = currentTime,
                CoinId = coin.Id
            };
            var result = await _dbRepository.Add(newPrice);
            await _dbRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}