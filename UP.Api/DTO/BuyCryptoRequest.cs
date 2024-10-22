namespace UP.DTO;

public class BuyCryptoRequest
{
    public BuyCryptoRequest(Guid userId, string coinName, double quantity)
    {
        UserId = userId;
        CoinName = coinName;
        Quantity = quantity;
    }

    public Guid UserId { get; set; }
    public string CoinName { get; set; }
    public double Quantity { get; set; }
}