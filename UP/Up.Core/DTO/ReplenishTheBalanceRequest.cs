namespace Up.Core.DTO;

public class ReplenishTheBalanceRequest
{
    public ReplenishTheBalanceRequest(Guid userId, double quantityUsd)
    {
        UserId = userId;
        QuantityUsd = quantityUsd;
    }

    public ReplenishTheBalanceRequest()
    {
    }

    public Guid UserId { get; set; }
    public double QuantityUsd { get; set; }
}