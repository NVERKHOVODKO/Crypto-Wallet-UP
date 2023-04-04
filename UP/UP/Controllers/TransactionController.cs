using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UP.Models;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController: ControllerBase
    {
        [HttpGet, Route("getUserConversationsHistory")]
        public async Task<ActionResult> GetUserList(int id)
        {
            var tr = new Repositories.TransactionsRepository();
            try
            {
                return Ok(tr.GetUserConversionsHistory(id));
            }
            catch(Exception)
            {
                return BadRequest("Unable to return user transactions history");
            }
        }
        
        [HttpGet, Route("getUserDepositHistory")]
        public async Task<ActionResult> GetUserDepositHistory(int id)
        {
            try
            {
                var tr = new Repositories.TransactionsRepository();
                return Ok(tr.GetUserDepositHistory(id));            
            }
            catch(Exception)
            {
                return BadRequest("Unable to return user transactions history");
            }
        }
        
        [HttpPost, Route("convert")]
        public async Task<ActionResult> Convert(string shortNameStart, string shortNameFinal, double quantity, int userId)
        {
            try
            {
                if (quantity == 0)
                {
                    return BadRequest("Error. Quantity must be above than zero");
                }
                
                // TODO govnokod
                //double priceRatio = await GetPriceRatio(shortNameStart, shortNameFinal);
                
                string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
                string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + shortNameStart + "&tsyms=" + shortNameFinal;
                var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseContent);

                double priceRatio =   (double)json[shortNameFinal.ToUpper()];
                
                //double priceRatio = 1;
                double finalQuantity = priceRatio * quantity;
                var ur = new Repositories.UserRepository();
                double startCoinQuantityInUserWallet = ur.GetCoinQuantityInUserWallet(userId, shortNameStart);
                if (startCoinQuantityInUserWallet < quantity)
                {
                    return BadRequest("The user doesn't have enough coins to complete the conversion");
                }
                var cr = new Repositories.CurrencyRepository();
                cr.SubtractCoinFromUser(userId, shortNameStart, quantity);
                cr.AddCryptoToUserWallet(userId, shortNameFinal, finalQuantity);
                var tr = new Repositories.TransactionsRepository();
                tr.WriteNewConversionDataToDatabase(new Conversion(1, 0, quantity, finalQuantity, await cr.GetCoinPrice(quantity, shortNameStart), shortNameStart, shortNameFinal, userId, DateTime.Now));
                return Ok("Converted successfully");
            }
            catch (Exception e)
            {
                return BadRequest("Error. Currencies have not been converted");
            }
        }
        

        /*public async Task<double> GetPriceRatio(string shortNameStart, string shortNameFinal)
        {
            string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + shortNameStart + "&tsyms=" + shortNameFinal;
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseContent);

            return  (double)json[shortNameFinal.ToUpper()];
        }*/
        
        
        [HttpPost, Route("buyCrypto")]
        public async Task<ActionResult> BuyCrypto(int userId, string coinName, double quantity)
        {
            try
            {
                if (quantity == 0)
                {
                    return UnprocessableEntity("Quantity must be above than zero");
                }
                var ur = new Repositories.UserRepository();
                var cr = new Repositories.CurrencyRepository();
                double quantityUSDTInUserWallet = ur.GetCoinQuantityInUserWallet(userId, "usdt");
                if (quantityUSDTInUserWallet < await cr.GetCoinPrice(quantity, coinName))
                {
                    return UnprocessableEntity("Not enough balance");
                }
                cr.SubtractCoinFromUser(userId, "usdt", await cr.GetCoinPrice(quantity, coinName));
                cr.AddCryptoToUserWallet(userId, coinName, quantity);
                return Ok("Transaction completed successfully");
            }
            catch (Exception e)
            {
                return BadRequest("Transaction wasn't completed");
            }
        }
        
        
        [HttpPut, Route("sellCrypto")]
        public async Task<ActionResult> SellCrypto(int userId, string coinName, double quantityForSell)
        {
            try
            {
                if (quantityForSell == 0)
                {
                    return UnprocessableEntity("Quantity must be above than zero");
                }
                var ur = new Repositories.UserRepository();
                var cr = new Repositories.CurrencyRepository();
                double quantityInUserWallet = ur.GetCoinQuantityInUserWallet(userId, coinName);
                if (quantityInUserWallet < quantityForSell)
                {
                    return UnprocessableEntity("Not enough coins");
                }
                cr.SubtractCoinFromUser(userId, coinName, quantityForSell);
                cr.AddCryptoToUserWallet(userId, "usdt", await cr.GetCoinPrice(quantityForSell, coinName));
                return Ok("Transaction completed successfully");
            }
            catch (Exception e)
            {
                return BadRequest("Transaction wasn't completed");
            }
        }
        
        [HttpPost, Route("sendCrypto")]
        public async Task<ActionResult> SendCrypto(int receiverId, int senderId, string coinName, double quantityForSend)
        {
            try
            {
                if (quantityForSend == 0)
                {
                    return UnprocessableEntity("Quantity must be above than zero");
                }
                var ur = new Repositories.UserRepository();
                var cr = new Repositories.CurrencyRepository();
                double quantityInUserWallet = ur.GetCoinQuantityInUserWallet(senderId, coinName);
                if (quantityInUserWallet < quantityForSend)
                {
                    return UnprocessableEntity("Not enough coins");
                }
                cr.SubtractCoinFromUser(senderId, coinName, quantityForSend);
                cr.AddCryptoToUserWallet(receiverId, coinName, quantityForSend);
                cr.WriteTransactionToDatabase(coinName, quantityForSend, senderId, receiverId);
                return Ok("Transfer completed successfully");
            }
            catch (Exception e)
            {
                return BadRequest("Transfer wasn't completed");
            }
        }
        
        [HttpPost, Route("replenishTheBalance")]
        public async Task<ActionResult> ReplenishTheBalance(int userId, double quantityUsd)
        {
            var tr = new Repositories.TransactionsRepository();
            try
            {
                tr.ReplenishTheBalance(userId, quantityUsd);
                return Ok("Balance replenished successfully");
            }
            catch(Exception)
            {
                return BadRequest("Unable to replenish the balance");
            }
        }
        
        [HttpPut, Route("withdrawUSDT")]// TODO how return error not enoght crypto
        public async Task<ActionResult> WithdrawUSDT(int userId, double quantityUsd)
        {
            var tr = new Repositories.TransactionsRepository();
            try
            {
                tr.WithdrawUSDT(userId, quantityUsd);
                return Ok("Transaction was successful");
            }
            catch(Exception)
            {
                return BadRequest("Unable to withdraw the balance");
            }
        }
        
        [HttpGet, Route("getUserWithdrawalsHistory")]
        public async Task<ActionResult> GetUserWithdrawalsHistory(int userId)
        {
            var tr = new Repositories.TransactionsRepository();
            try
            {
                return Ok(tr.GetUserWithdrawalsHistory(userId));
            }
            catch(Exception)
            {
                return BadRequest("Unable to get user withdrawals history");
            }
        }
    }
}