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
                string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
                using var httpClient = new HttpClient();

                Dictionary<string, string> cryptoDictionary = CoinList.GetCryptoDictionary();
                var coins = new List<CoinsInformation>();

                int i = 0;
                foreach (string key in cryptoDictionary.Keys)
                {
                    Console.WriteLine("Криптовалюта {0} имеет полное название {1}", key, cryptoDictionary[key]);
                    string shortName = key;
                    string fullName = cryptoDictionary[key];
                    var cr = new Repositories.CurrencyRepository();
                    double price = await cr.GetCoinPrice(1, key);
                    coins.Add(new CoinsInformation(i, fullName, shortName, @"C:\НЕ СИСТЕМА\BSUIR\второй курс\UP\cryptoicons_png\128\" + key, 0, 0, price));
                    i++;
                }
                return Ok(coins);
            }
            catch (Exception e)
            {
                return BadRequest("Error");
            }
        }
    }
}