namespace Up.Core.Models;

public class RestorePasswordCodeModel : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string Code { get; set; }

    [Required] public string Email { get; set; }
}