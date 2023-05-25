namespace UP.Models;

public class CoinList
{
    public static Dictionary<string, string> cryptoDictionary = new Dictionary<string, string>
    {
        {"btc", "Bitcoin"},
        {"eth", "Ethereum"},
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
        {"fet", "Fetch.ai"}
    };

    public static Dictionary<string, string> GetCryptoDictionary()
    {
        return cryptoDictionary;
    }

    public CoinList()
    {
    }
}