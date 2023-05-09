using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UP.Models;
using UP.Models.Base;
using UP.Repositories;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ILogger<CurrencyController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet, Route("getUserCoins")]
        public async Task<ActionResult> GetUserCoins(int userId)
        {
            try
            {
                _logger.LogInformation($"Return user coinList. Id: " + userId);
                var ur = new UserRepository();
                return Ok(ur.GetUserCoins(userId));
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't return coinList");
            }
        }
        
        [HttpGet, Route("getUserCoinsFull")]
        public async Task<IActionResult> GetUserCoinsFull(int userId)
        {
            _logger.LogInformation($"Return user coinList. Id: " + userId);
            var ur = new UserRepository();
            return Ok(await ur.GetUserCoinsFull(userId));
            /*try
            {
                _logger.LogInformation($"Return user coinList. Id: " + userId);
                var ur = new UserRepository();
                return Ok(await ur.GetUserCoinsFull(userId));
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error. Can't return coinList");
                return BadRequest("Error. Can't return coinList");
            }*/
        }
        
        
        [HttpGet, Route("getQuantityAfterConversion")]
        public async Task<IActionResult> GetQuantityAfterConversion(string shortNameStart, string shortNameFinal, double quantity, int userId)
        {
            try
            {
                _logger.LogInformation($"User:" + userId + "Converted " + quantity + " " + shortNameStart + " to " + shortNameFinal);
                string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
                string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + shortNameStart + "&tsyms=" + shortNameFinal;
                var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseContent);
                double priceRatio =   (double)json[shortNameFinal.ToUpper()];
                double finalQuantity = priceRatio * quantity;
                var ur = new Repositories.UserRepository();
                _logger.LogInformation($"User:" + userId + "Converted " + quantity + "(" + shortNameStart + ") to " + shortNameFinal + "(" + finalQuantity + ")");
                return Ok(finalQuantity);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error. Currencies have not been converted");
                return BadRequest("Произошла неизвестная ошибка");
            }
        }
        
        
        [HttpGet, Route("getUserBalance")]
        public async Task<ActionResult> GetUserBalance(int userId)
        {
            try
            {
                var cr = new CurrencyRepository();
                double balance = await cr.GetUserBalance(userId);
                return Ok(balance);
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't count balance");
            }
        }
        
        [HttpGet, Route("getCoinQuantityInUserWallet")]
        public async Task<ActionResult> GetCoinQuantityInUserWallet(int userId, string coinName)
        {
            try
            {
                var ur = new UserRepository();
                double quantity = ur.GetCoinQuantityInUserWallet(userId, coinName);
                return Ok(quantity);
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't return quantity");
            }
        }
        
        
        [HttpGet, Route("getCoinPrice")]
        public async Task<ActionResult> GetCoinPrice(double quantity, string coinName)
        {
            try
            {
                var cr = new CurrencyRepository();
                double price = await cr.GetCoinPrice(quantity, coinName);
                return Ok(price);
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't get coin price");
            }
        }

        
        [HttpGet, Route("getCurrenciesList")]
        public async Task<ActionResult> GetCoinsList()
        {
            try
            {
                Dictionary<string, string> cryptoDictionary = CoinList.GetCryptoDictionary();
                var coins = new List<CoinsInformation>();
                int i = 0;
                var cr = new CurrencyRepository();
                var coin = new CoinsInformation();
                foreach (string key in cryptoDictionary.Keys)
                {
                    coin = await cr.GetFullCoinInformation(key);
                    coins.Add(new (i, coin.ShortName, coin.FullName, coin.IconPath, coin.DailyVolume, coin.DailyImpact, coin.Price, coin.PercentagePriceChangePerDay));
                    i++;
                }
                return Ok(coins);
            }catch (Exception e)
            {
                return BadRequest("Error");
            }
        }
    }
}