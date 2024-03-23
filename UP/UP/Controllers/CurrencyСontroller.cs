using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ProjectX.Exceptions;
using Repository;
using UP.DTO;
using UP.Models;
using UP.Models.Base;
using UP.Services.Interfaces;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<CurrencyController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IDbRepository _dbRepository;
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ILogger<CurrencyController> logger, IUserRepository userRepository,
        ICurrencyRepository currencyRepository, IDbRepository dbRepository, ICurrencyService currencyService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
        _dbRepository = dbRepository;
        _currencyService = currencyService;
    }

    [HttpGet]
    [Route("getUserCoins")]
    public async Task<ActionResult> GetUserCoins(Guid userId)
    {
        _logger.LogInformation("Return user coinList. Id: " + userId);
        return Ok(_userRepository.GetUserCoins(userId));
    }

    [HttpGet]
    [Route("getUserCoinsFull")]
    public async Task<IActionResult> GetUserCoinsFull(Guid userId)
    {
        _logger.LogInformation("Return user coinList. Id: " + userId);
        return Ok(await _userRepository.GetUserCoinsFull(userId));
    }


    [HttpGet]
    [Route("getQuantityAfterConversion")]
    public async Task<IActionResult> GetQuantityAfterConversion(string shortNameStart, string shortNameFinal,
        double quantity, Guid userId)
    {
        try
        {
            _logger.LogInformation("User:" + userId + "Converted " + quantity + " " + shortNameStart + " to " +
                                   shortNameFinal);
            var apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            var url = "https://min-api.cryptocompare.com/data/price?fsym=" + shortNameStart + "&tsyms=" +
                      shortNameFinal;
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseContent);
            var priceRatio = (double)json[shortNameFinal.ToUpper()];
            var finalQuantity = priceRatio * quantity;
            _logger.LogInformation("User:" + userId + "Converted " + quantity + "(" + shortNameStart + ") to " +
                                   shortNameFinal + "(" + finalQuantity + ")");
            return Ok(finalQuantity);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error. Currencies have not been converted");
            return BadRequest("Произошла неизвестная ошибка");
        }
    }


    [HttpGet]
    [Route("getUserBalance")]
    public async Task<ActionResult> GetUserBalance(Guid userId)
    {
        try
        {
            var balance = await _currencyRepository.GetUserBalance(userId);
            _logger.LogInformation("User balance: " + balance);
            return Ok(balance);
        }
        catch (Exception e)
        {
            return BadRequest("Не удалось вернуть баланс пользователя");
        }
    }

    [HttpGet]
    [Route("getCoinQuantityInUserWallet")]
    public async Task<ActionResult> GetCoinQuantityInUserWallet(Guid userId, string coinName)
    {
        try
        {
            var quantity = _userRepository.GetCoinQuantityInUserWallet(userId, coinName);
            return Ok(quantity);
        }
        catch (Exception e)
        {
            return BadRequest("Не удалось вернуть количество монет");
        }
    }
    
    [HttpGet]
    [Route("getCoinPrice")]
    public async Task<ActionResult> GetCoinPrice(double quantity, string coinName)
    {
        try
        {
            var price = await _currencyRepository.GetCoinPrice(quantity, coinName);
            return Ok(price);
        }
        catch (Exception e)
        {
            return BadRequest("Не удалось вернуть цену");
        }
    }
    
    [HttpGet]
    [Route("get-coin-price-history/{shortName}")]
    public async Task<ActionResult> GetCoinRatio(string shortName)
    {
        var coinSnapShots = await _dbRepository.Get<CryptoCurrencyPrices>()
            .Where(c => c.CoinShortName.Equals(shortName, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync();
        if (coinSnapShots == null)
            throw new EntityNotFoundException($"Coin with short name {shortName} not found.");
        
        return Ok(coinSnapShots);
    }

    [HttpGet]
    [Route("getCurrenciesList")]
    public async Task<ActionResult> GetCoinsList()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .Where(x => x.IsActive == true)
            .ToListAsync();

        var cryptoDictionary = coinsList.ToDictionary(
            coin => coin.ShortName.ToLower(),
            coin => coin.FullName.ToLower()
        );
        var coins = new List<CoinsInformation>();
        foreach (var key in cryptoDictionary.Keys)
        {
            var temp = await _currencyRepository.GetFullCoinInformation(key);
            coins.Add(new CoinsInformation
            {
                Id = temp.Id,
                ShortName = temp.ShortName,
                FullName = temp.FullName,
                IconPath = temp.IconPath,
                DailyVolume = temp.DailyVolume,
                DailyImpact = temp.DailyImpact,
                Price = temp.Price,
                PercentagePriceChangePerDay = temp.PercentagePriceChangePerDay,
                Quantity = temp.Quantity
            });
            
            var coin = await _dbRepository.Get<CoinListInfo>()
                .FirstOrDefaultAsync(c => c.ShortName.ToLower() == temp.FullName.ToLower());
            if (coin == null)
                throw new EntityNotFoundException($"Coin with short name {temp.FullName} not found.");

            var currentTime = DateTime.UtcNow;
            var tenMinutesAgo = currentTime.AddMinutes(-1);

            var existingPrice = _dbRepository.Get<CryptoCurrencyPrices>()
                .FirstOrDefault(p => p.CoinId == coin.Id && p.Timestamp >= tenMinutesAgo);

            if (existingPrice != null) continue;
            var newPrice = new CryptoCurrencyPrices
            {
                Id = Guid.NewGuid(),
                CoinShortName = temp.FullName,
                Price = temp.Price,
                Timestamp = currentTime,
                CoinId = coin.Id
            };
            var result = await _dbRepository.Add(newPrice);
        }
        await _dbRepository.SaveChangesAsync();
        return Ok(coins);
    }
}