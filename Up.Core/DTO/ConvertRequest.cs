namespace Up.Core.DTO;

public class ConvertRequest
{
    public string ShortNameStart { get; set; }
    public string ShortNameFinal { get; set; }
    public double Quantity { get; set; }
    public Guid UserId { get; set; }
}