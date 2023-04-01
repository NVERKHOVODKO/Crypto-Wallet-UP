using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace UP.Repositories;

public class AuthorizationRepository: RepositoryBase
{
    public string GetSalt()
    {
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    public string Hash(string inputString)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    
    public bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        password = Hash(password);
        password = Hash(password + salt);
        return password == hashedPassword;
    }

    public bool IsValidEmail(string email)
    {
        // Шаблон для проверки email
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Создание объекта регулярного выражения
        Regex regex = new Regex(pattern);

        // Проверка соответствия email шаблону
        return regex.IsMatch(email);
    }
}