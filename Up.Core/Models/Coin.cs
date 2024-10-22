using System.ComponentModel.DataAnnotations;

namespace Up.Core.Models;

public class Coin : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public double Quantity { get; set; }

    [Required] public string Shortname { get; set; }
}