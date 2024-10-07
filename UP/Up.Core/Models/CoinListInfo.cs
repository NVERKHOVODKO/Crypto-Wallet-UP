using System.ComponentModel.DataAnnotations;

namespace Up.Core.Models;

public class CoinListInfo : BaseModel
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string ShortName { get; set; }
    
    [Required] public string FullName { get; set; }
    
    [Required] public bool IsActive { get; set; }
}