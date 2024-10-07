namespace Up.Core.DTO;

public class GetCoinRatio
{
    public string CoinShortName { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}