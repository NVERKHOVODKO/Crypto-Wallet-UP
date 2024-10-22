namespace Up.Core.Handlers;

public class IPHandler
{
    public static string GetIpAddress()
    {
        try
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                .ToString();

            return ipAddress ?? "IP address not found";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}