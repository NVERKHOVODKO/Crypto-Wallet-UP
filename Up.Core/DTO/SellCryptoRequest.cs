namespace Up.Core.DTO;

public class SellCryptoRequest
{
    public SellCryptoRequest(Guid userId, string coinName, double quantityForSell)
    {
        UserId = userId;
        CoinName = coinName;
        QuantityForSell = quantityForSell;
    }

    public Guid UserId { get; set; }
    public string CoinName { get; set; }
    public double QuantityForSell { get; set; }
}