namespace Up.Core.Models;

public class BaseModel : IEntity
{
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public DateTime? DateUpdated { get; set; }

    public Guid Id { get; set; }
}