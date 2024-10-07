namespace Up.Core.Models;

public class LoginHistory : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string IPAddress { get; set; }

    [Required] public Guid UserId { get; set; }

    public virtual User User { get; set; }

    public static string GetIPAddress()
    {
        var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
        return ipAddress;
    }
}