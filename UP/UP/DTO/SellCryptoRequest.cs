namespace UP.DTO;

public class SellCryptoRequest
{
    public int UserId { get; set; }
    public string CoinName { get; set; }
    public double QuantityForSell { get; set; }

    public SellCryptoRequest(int userId, string coinName, double quantityForSell)
    {
        UserId = userId;
        CoinName = coinName;
        QuantityForSell = quantityForSell;
    }
}