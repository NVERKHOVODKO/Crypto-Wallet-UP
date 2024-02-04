using System.ComponentModel.DataAnnotations;
using Entities;

namespace UP.ModelsEF;

public class Coin : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public double Quantity { get; set; }

    [Required] public string Shortname { get; set; }
}