namespace UP.DTO;

public class ReplenishTheBalanceRequest
{
    public int UserId { get; set; }
    public int QuantityUsd { get; set; }

    public ReplenishTheBalanceRequest(int userId, int quantityUsd)
    {
        UserId = userId;
        QuantityUsd = quantityUsd;
    }

    public ReplenishTheBalanceRequest()
    {
    }
}