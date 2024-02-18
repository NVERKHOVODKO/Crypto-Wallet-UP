using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace UP.Repositories;

public class AuthorizationRepository : RepositoryBase
{
    public string GetSalt()
    {
        var salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        return Convert.ToBase64String(salt);
    }

    private string Hash(string inputString)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        password = Hash(password);
        password = Hash(password + salt);
        return password == hashedPassword;
    }

    public bool IsValidEmail(string email)
    {
        const string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        return Regex.Match(email, pattern).Success;
    }
}