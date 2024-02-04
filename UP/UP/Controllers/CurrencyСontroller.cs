using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Repository;
using UP.Models;
using UP.Models.Base;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<CurrencyController> _logger;
    private readonly IUserRepository _userRepository;

    public CurrencyController(ILogger<CurrencyController> logger, IUserRepository userRepository,
        ICurrencyRepository currencyRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
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
    [Route("getCurrenciesList")]
    public async Task<ActionResult> GetCoinsList()
    {
        try
        {
            var cryptoDictionary = CoinList.GetCryptoDictionary();
            var coins = new List<CoinsInformation>();
            var i = 0;
            var temp = new CoinsInformation();
            foreach (var key in cryptoDictionary.Keys)
            {
                temp = await _currencyRepository.GetFullCoinInformation(key);
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
                i++;
            }

            return Ok(coins);
        }
        catch (Exception e)
        {
            return BadRequest("Произошла неивестная ошибка");
        }
    }
}