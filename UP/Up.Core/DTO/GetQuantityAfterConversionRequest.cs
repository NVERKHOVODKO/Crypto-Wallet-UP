namespace Up.Core.DTO;

public class GetQuantityAfterConversionRequest
{
    public GetQuantityAfterConversionRequest(string shortNameStart, string shortNameFinal, double quantity, int userId)
    {
        ShortNameStart = shortNameStart;
        ShortNameFinal = shortNameFinal;
        Quantity = quantity;
        UserId = userId;
    }

    public string ShortNameStart { get; set; }
    public string ShortNameFinal { get; set; }
    public double Quantity { get; set; }
    public int UserId { get; set; }
}