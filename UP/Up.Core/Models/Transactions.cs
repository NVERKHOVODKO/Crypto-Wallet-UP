namespace Up.Core.Models;

public class Transactions : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string CoinName { get; set; }

    [Required] public double Quantity { get; set; }

    [Required] public Guid SenderId { get; set; }

    [Required] public Guid ReceiverId { get; set; }

    public virtual User Sender { get; set; }
    public virtual User Receiver { get; set; }
}