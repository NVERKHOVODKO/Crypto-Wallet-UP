namespace Up.Core.Handlers;

public class HashHandler
{
    public static string GenerateSalt(int size)
    {
        using var rng = new RNGCryptoServiceProvider();
        var saltBytes = new byte[size];
        rng.GetBytes(saltBytes); // Заполняем массив байтов случайными данными.
        return Convert.ToBase64String(saltBytes); // Преобразуем в строку в формате Base64.
    }

    public static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        // Хешируем пароль.
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var passwordHash = sha256.ComputeHash(passwordBytes);

        // Преобразуем соль из Base64 обратно в байты.
        var saltBytes = Convert.FromBase64String(salt);

        // Создаем массив, объединяя хеш пароля и соль.
        var combinedBytes = new byte[passwordHash.Length + saltBytes.Length];
        Array.Copy(passwordHash, 0, combinedBytes, 0, passwordHash.Length);
        Array.Copy(saltBytes, 0, combinedBytes, passwordHash.Length, saltBytes.Length);

        // Хешируем объединенный массив.
        var hashedBytes = sha256.ComputeHash(combinedBytes);

        // Преобразуем хеш в строку в формате Base64.
        return Convert.ToBase64String(hashedBytes);
    }
}