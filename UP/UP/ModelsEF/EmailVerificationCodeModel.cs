using System.ComponentModel.DataAnnotations;
using Entities;

namespace UP.ModelsEF;

public class EmailVerificationCodeModel : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string Code { get; set; }
    
    [Required] public string Email { get; set; }
}