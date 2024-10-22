namespace Up.Core.Models;

public class UsersCoins : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] public Guid CoinId { get; set; }

    public virtual User User { get; set; }
    public virtual Coin Coin { get; set; }
}