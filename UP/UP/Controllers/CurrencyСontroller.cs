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

        /*[HttpGet, Route("Currencies list")]
        public async Task<ActionResult> GetCoinsList()
        {
            try
            {
                string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
                using var httpClient = new HttpClient();
                string[] coinNamesShort = {"BTC", "ETH", "ADA", "BNB", "XRP", "SOL", "DOT", "DOGE", "LUNA", "AVAX", "ALGO", "ATOM", "FIL", "UNI", "LINK"};
                string[] coinNamesFull = {
                    "Bitcoin",
                    "Ethereum",
                    "Cardano",
                    "Binance Coin",
                    "XRP",
                    "Solana",
                    "Polkadot",
                    "Dogecoin",
                    "Terra",
                    "Avalanche",
                    "Algorand",
                    "Cosmos",
                    "Filecoin",
                    "Uniswap",
                    "Chainlink"
                };
                List<Models.Coin> coins = new List<Coin>();
                Models.Coin coin = new Coin();
                for (int i = 0; i < coinNamesShort.Length; i++)
                {
                    httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
                    string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + coinNamesShort[i] + "&tsyms=USD";
                    //string url = $"https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + coinNamesShort[i] + "&tsyms=USD";

                    var response = await httpClient.GetAsync(url);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    JObject json = JObject.Parse(responseContent);
                    double price = (double)json["USD"];
                    coins.Add(new Coin(i, coinNamesFull[i], coinNamesShort[i], price, "C:\\НЕ СИСТЕМА\\BSUIR\\второй курс\\UP\\cryptoicons_png\\128\\" + coinNamesShort[i].ToLower(), 0, 0));
                }
                return Ok(coins);
            }
            catch (Exception e)
            {
                return BadRequest("Error");
            }
        }*/
    }
}