using System.Security.Cryptography;
using System.Text;
using Api.OpenAI.Handlers.Interfaces;

namespace Api.OpenAI.Handlers;

public class HashHelpers : IHashHelpers
{
    public string GenerateSalt(int size)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }

    public string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var passwordHash = sha256.ComputeHash(passwordBytes);

            var saltBytes = Convert.FromBase64String(salt);

            var combinedBytes = new byte[passwordHash.Length + saltBytes.Length];
            Array.Copy(passwordHash, 0, combinedBytes, 0, passwordHash.Length);
            Array.Copy(saltBytes, 0, combinedBytes, passwordHash.Length, saltBytes.Length);

            var hashedBytes = sha256.ComputeHash(combinedBytes);

            return Convert.ToBase64String(hashedBytes);
        }
    }
}