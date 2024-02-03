namespace UP.DTO;

public class SellCryptoRequest
{
    public Guid UserId { get; set; }
    public string CoinName { get; set; }
    public double QuantityForSell { get; set; }

    public SellCryptoRequest(Guid userId, string coinName, double quantityForSell)
    {
        UserId = userId;
        CoinName = coinName;
        QuantityForSell = quantityForSell;
    }
}