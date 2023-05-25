namespace UP.DTO;

public class ReplenishTheBalanceRequest
{
    public int UserId { get; set; }
    public double QuantityUsd { get; set; }

    public ReplenishTheBalanceRequest(int userId, double quantityUsd)
    {
        UserId = userId;
        QuantityUsd = quantityUsd;
    }

    public ReplenishTheBalanceRequest()
    {
    }
}