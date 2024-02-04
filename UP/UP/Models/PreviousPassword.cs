using System.ComponentModel.DataAnnotations;
using Entities;

namespace UP.ModelsEF;

public class PreviousPassword : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] public string PreviousPasswordHashed { get; set; }

    public virtual User User { get; set; }
}