using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using UP.Models;
using Newtonsoft.Json.Linq;
using UP.Models.Base;
using UP.Repositories;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        [HttpGet, Route("getUserCoins")]
        public async Task<ActionResult> GetUserCoins(int userId)
        {
            try
            {
                var ur = new Repositories.UserRepository();
                return Ok(ur.GetUserCoins(userId));
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't return coinList");
            }
        }
        
        
        [HttpGet, Route("getUserBalance")]
        public async Task<ActionResult> GetUserBalance(int userId)
        {
            try
            {
                var cr = new Repositories.CurrencyRepository();
                double balance = await cr.GetUserBalance(userId);
                return Ok(balance);
            }
            catch (Exception e)
            {
                return BadRequest("Error. Can't count balance");
            }
        }
        
        
        [HttpGet, Route("getCoinPrice")]
        public async Task<ActionResult> GetCoinPrice(double quantity, string coinName)
        {
            try
            {
                var cr = new Repositories.CurrencyRepository();
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
                var cr = new Repositories.CurrencyRepository();
                CoinsInformation coin = new CoinsInformation();
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