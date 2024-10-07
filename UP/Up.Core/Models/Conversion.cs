namespace Up.Core.Models;

public class Conversion : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public double Commission { get; set; }

    [Required] public double BeginCoinQuantity { get; set; }

    [Required] public double EndCoinQuantity { get; set; }

    [Required] public double QuantityUSD { get; set; }

    [Required] public string BeginCoinShortname { get; set; }

    [Required] public string EndCoinShortname { get; set; }

    [Required] public Guid UserId { get; set; }

    public virtual User User { get; set; }
}