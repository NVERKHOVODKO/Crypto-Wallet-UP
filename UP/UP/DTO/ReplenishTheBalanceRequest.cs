namespace UP.DTO;

public class ReplenishTheBalanceRequest
{
    public Guid UserId { get; set; }
    public double QuantityUsd { get; set; }

    public ReplenishTheBalanceRequest(Guid userId, double quantityUsd)
    {
        UserId = userId;
        QuantityUsd = quantityUsd;
    }

    public ReplenishTheBalanceRequest()
    {
    }
}