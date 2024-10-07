namespace Up.Core.Models;

public class CoinList
{
    /*private readonly IDbRepository _dbRepository;

    public CoinList(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }*/

    private static readonly Dictionary<string, string> CryptoDictionary = new()
    {
        { "btc", "Bitcoin" },
        { "eth", "Ethereum" },
        {"usdt", "Tether"},
        {"bnb", "Binance Coin"},
        {"sol", "Solana"},
        {"ada", "Cardano"},
        {"xrp", "XRP"},
        {"dot", "Polkadot"},
        {"doge", "Dogecoin"},
        {"uni", "Uniswap"},
        {"luna", "Terra"},
        {"link", "Chainlink"},
        {"avax", "Avalanche"},
        {"matic", "Polygon"},
        {"shib", "Shiba Inu"},
        {"atom", "Cosmos"},
        {"fil", "Filecoin"},
        {"xtz", "Tezos"},
        {"ltc", "Litecoin"},
        {"ftt", "FTX Token"},
        {"algo", "Algorand"},
        {"vet", "VeChain"},
        {"eos", "EOS"},
        {"trb", "Tellor"},
        {"ksm", "Kusama"},
        {"cake", "PancakeSwap"},
        {"tfuel", "Theta Fuel"},
        {"sushi", "SushiSwap"},
        {"dcr", "Decred"},
        { "fet", "Fetch.ai" }
    };

    /*public async Task<Dictionary<string, string>> GetActiveCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .Where(x => x.IsActive)
            .ToListAsync();

        var coins = coinsList.ToDictionary(
            coin => coin.ShortName.ToLower(),
            coin => coin.FullName.ToLower()
        );

        return coins;
    }*/
}