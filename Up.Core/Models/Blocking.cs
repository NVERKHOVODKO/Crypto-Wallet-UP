
namespace Up.Core.Models;

public class Blocking : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string Cause { get; set; }

    [Required] public Guid UserId { get; set; }

    public virtual User User { get; set; }
}