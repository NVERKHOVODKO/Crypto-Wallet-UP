using System.ComponentModel.DataAnnotations;
using Entities;

namespace UP.ModelsEF;

public class RestorePasswordCodeModel : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string Code { get; set; }

    [Required] public string Email { get; set; }
}