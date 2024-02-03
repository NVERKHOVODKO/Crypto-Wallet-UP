using System.Net;

namespace Analitique.BackEnd.Handlers;

public class IPHandler
{
    public static string GetIpAddress()
    {
        try
        {
            string? ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                .ToString();

            return ipAddress ?? "IP address not found";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}