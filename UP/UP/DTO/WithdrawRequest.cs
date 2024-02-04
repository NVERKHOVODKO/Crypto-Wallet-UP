namespace UP.DTO;

public class WithdrawRequest
{
    public Guid UserId { get; set; }
    public double QuantityForWithdraw { get; set; }
}