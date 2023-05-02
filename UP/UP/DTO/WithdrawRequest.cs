namespace UP.DTO;

public class WithdrawRequest
{
    public int UserId { get; set; }
    public double QuantityForWithdraw { get; set; }

    public WithdrawRequest(int userId, double quantityForWithdraw)
    {
        UserId = userId;
        QuantityForWithdraw = quantityForWithdraw;
    }
} 