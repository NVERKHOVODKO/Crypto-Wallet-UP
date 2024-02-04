using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Repository;
using UP.DTO;
using UP.ModelsEF;
using UP.Repositories;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IDbRepository _dbRepository;
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IUserRepository _userRepository;

    public TransactionController(IDbRepository dbRepository, ILogger<TransactionController> logger,
        IUserRepository userRepository, ICurrencyRepository currencyRepository,
        ITransactionsRepository transactionsRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
        _transactionsRepository = transactionsRepository;
        _dbRepository = dbRepository;
    }

    [HttpGet]
    [Route("getCoinQuantity/{coinName}/{quantityUSD}")]
    public async Task<ActionResult> GetCoinQuantity(string coinName, double quantityUSD)
    {
        return Ok(await _currencyRepository.GetCoinQuantity(quantityUSD, coinName));
    }

    [HttpGet]
    [Route("getUserConversationsHistory/{id}")]
    public async Task<ActionResult> GetUserList(Guid id)
    {
        return Ok(_transactionsRepository.GetUserConversionsHistory(id));
    }

    [HttpGet]
    [Route("getUserDepositHistory/{id}")]
    public async Task<ActionResult> GetUserDepositHistory(Guid id)
    {
        return Ok(_transactionsRepository.GetUserDepositHistory(id));
    }

    [HttpPost]
    [Route("convert")]
    public async Task<ActionResult> Convert([FromBody] ConvertRequest request)
    {
        _logger.LogInformation("User:" + request.UserId + "Converted " + request.Quantity + " " +
                               request.ShortNameStart + " to " + request.ShortNameFinal);
        if (request.Quantity == 0)
        {
            _logger.LogInformation("Error. Quantity must be above than zero");
            return BadRequest("Количество должно быть больше нуля");
        }

        if (request.Quantity < 0)
        {
            _logger.LogInformation("Error. Quantity must be above than zero");
            return BadRequest("Количество должно быть больше нуля");
        }

        var apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
        var url = "https://min-api.cryptocompare.com/data/price?fsym=" + request.ShortNameStart + "&tsyms=" +
                  request.ShortNameFinal;
        var response = await httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseContent);
        var priceRatio = (double)json[request.ShortNameFinal.ToUpper()];
        var finalQuantity = priceRatio * request.Quantity;
        var startCoinQuantityInUserWallet =
            _userRepository.GetCoinQuantityInUserWallet(request.UserId, request.ShortNameStart);
        if (startCoinQuantityInUserWallet < request.Quantity)
        {
            _logger.LogInformation("The user doesn't have enough coins to complete the conversion");
            return BadRequest("Недостаточно монет для совершения конвертации");
        }

        _currencyRepository.SubtractCoinFromUser(request.UserId, request.ShortNameStart, request.Quantity);
        _currencyRepository.AddCryptoToUserWallet(request.UserId, request.ShortNameFinal, finalQuantity);

        var conversionData = new Conversion
        {
            Id = Guid.Empty,
            Commission = 0,
            BeginCoinQuantity = request.Quantity,
            EndCoinQuantity = finalQuantity,
            QuantityUSD = await _currencyRepository.GetCoinPrice(request.Quantity, request.ShortNameStart),
            BeginCoinShortname = request.ShortNameStart,
            EndCoinShortname = request.ShortNameFinal,
            UserId = request.UserId
            
        };

        await _dbRepository.Add(conversionData);
        await _dbRepository.SaveChangesAsync();

        _logger.LogInformation("Converted successfully");
        return Ok("Конвертация совершена испешно");
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


    [HttpPost]
    [Route("buyCrypto")]
    public async Task<ActionResult> BuyCrypto([FromBody] BuyCryptoRequest request)
    {
        _logger.LogInformation("UserId:" + request.UserId + "\nCoin quantity:" + request.Quantity + "\nCoin name: " +
                               request.CoinName);
        if (request.Quantity == 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return UnprocessableEntity("Количество должно быть больше нуля");
        }

        var quantityUSDTInUserWallet = _userRepository.GetCoinQuantityInUserWallet(request.UserId, "usdt");
        if (quantityUSDTInUserWallet < request.Quantity)
        {
            _logger.LogInformation("Not enough balance");
            return UnprocessableEntity("Недостаточно монет");
        }

        _currencyRepository.SubtractCoinFromUser(request.UserId, "usdt", request.Quantity);
        var coinQuantity = await _currencyRepository.GetCoinQuantity(request.Quantity, request.CoinName);
        _currencyRepository.AddCryptoToUserWallet(request.UserId, request.CoinName, coinQuantity);
        _logger.LogInformation("UserId(" + request.UserId + ") bought " + coinQuantity + " " + request.CoinName);
        return Ok("Транзакция совершена успешно");
    }


    [HttpPut]
    [Route("sellCrypto")]
    public async Task<ActionResult> SellCrypto([FromBody] SellCryptoRequest request)
    {
        if (request.QuantityForSell == 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return UnprocessableEntity("Количество должно быть больше нуля");
        }

        if (request.QuantityForSell < 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return BadRequest("Количество должно быть больше нуля");
        }

        var quantityInUserWallet = _userRepository.GetCoinQuantityInUserWallet(request.UserId, request.CoinName);
        if (quantityInUserWallet < request.QuantityForSell)
        {
            _logger.LogInformation("Not enough coins");
            return UnprocessableEntity("Недостаточно монет");
        }

        _currencyRepository.SubtractCoinFromUser(request.UserId, request.CoinName, request.QuantityForSell);
        _currencyRepository.AddCryptoToUserWallet(request.UserId, "usdt",
            await _currencyRepository.GetCoinPrice(request.QuantityForSell, request.CoinName));
        return Ok("Транзакция совершена успешно");
    }

    [HttpPost]
    [Route("sendCrypto")]
    public async Task<ActionResult> SendCrypto([FromBody] SendCryptoRequest request)
    {
        _logger.LogInformation("Sended: " + request.QuantityForSend + " " + request.CoinName + " by user( " +
                               request.SenderId + ") to user(" + request.ReceiverId + ")");
        if (request.ReceiverId == request.SenderId)
        {
            _logger.LogInformation("You can't send cryptocurrency to yourself");
            return UnprocessableEntity("Невозможно отправить криптовалюту себе же");
        }

        if (request.QuantityForSend == 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return UnprocessableEntity("Количество должно быть больше нуля");
        }

        if (request.QuantityForSend < 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return UnprocessableEntity("Количество должно быть больше нуля");
        }

        var quantityInUserWallet = _userRepository.GetCoinQuantityInUserWallet(request.SenderId, request.CoinName);
        if (quantityInUserWallet < request.QuantityForSend)
        {
            _logger.LogInformation("Not enough coins");
            return UnprocessableEntity("Недостаточно монет");
        }

        _currencyRepository.SubtractCoinFromUser(request.SenderId, request.CoinName, request.QuantityForSend);
        _currencyRepository.AddCryptoToUserWallet(request.ReceiverId, request.CoinName, request.QuantityForSend);
        _currencyRepository.WriteTransactionToDatabase(request.CoinName, request.QuantityForSend, request.SenderId,
            request.ReceiverId);
        _logger.LogInformation("Transfer completed successfully");
        return Ok("Перевод выполнен успешно");
    }


    [HttpPost]
    [Route("replenishTheBalance")]
    public async Task<ActionResult> ReplenishTheBalance([FromBody] ReplenishTheBalanceRequest request)
    {
        _logger.LogInformation("Replenishment from user(" + request.UserId + "): " + request.QuantityUsd + "$");
        if (request.QuantityUsd == null) return BadRequest("Количество должно быть больше нуля");
        if (request.QuantityUsd == 0) return BadRequest("Количество должно быть больше нуля");
        if (request.QuantityUsd < 0) return BadRequest("Количество должно быть больше нуля");
        _transactionsRepository.ReplenishTheBalance(request.UserId, request.QuantityUsd);
        _logger.LogInformation("Balance replenished successfully");
        return Ok("Баланс пополнен успешно");
    }

    [HttpPut]
    [Route("withdrawUSDT")]
    public async Task<ActionResult> WithdrawUSDT([FromBody] WithdrawRequest request)
    {
        _logger.LogInformation("Withdraw from user(" + request.UserId + "): " + request.QuantityForWithdraw + "$");
        if (request.QuantityForWithdraw == 0) return UnprocessableEntity("Количество должно быть больше нуля");
        if (request.QuantityForWithdraw < 0)
        {
            _logger.LogInformation("Quantity must be above than zero");
            return UnprocessableEntity("Количество должно быть больше нуля");
        }

        var quantityInUserWallet = _userRepository.GetCoinQuantityInUserWallet(request.UserId, "usdt");
        if (quantityInUserWallet < request.QuantityForWithdraw)
        {
            _logger.LogInformation("Not enough balance");
            return UnprocessableEntity("Недостаточно монет");
        }

        _currencyRepository.SubtractCoinFromUser(request.UserId, "usdt", request.QuantityForWithdraw);
        _currencyRepository.WriteWithdrawToDatabase(request.QuantityForWithdraw, request.QuantityForWithdraw * 0.02,
            request.UserId);
        return Ok("Транзакция выполнена успешно");
    }

    [HttpGet]
    [Route("getUserWithdrawalsHistory/{userId}")]
    public async Task<IActionResult> GetUserWithdrawalsHistory(Guid userId)
    {
        return Ok(_transactionsRepository.GetUserWithdrawalsHistory(userId));
    }


    [HttpGet]
    [Route("getUserTransactionsHistory/{userId}")]
    public async Task<IActionResult> GetUserTransactionsHistory(Guid userId)
    {
        return Ok(_transactionsRepository.GetUserTransactionsHistory(userId));
    }
}