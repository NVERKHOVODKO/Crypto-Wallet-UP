using Microsoft.AspNetCore.Mvc;
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