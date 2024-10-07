namespace Up.Core.Models;

public class CryptoCurrencyPrices : BaseModel
{
    [Key] public Guid Id { get; set; }
    
    public string CoinShortName { get; set; }
    
    public double Price { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    [Required] public Guid CoinId { get; set; }
    
    public virtual CoinListInfo Coin { get; set; }
}