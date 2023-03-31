using System;
using System.Text.RegularExpressions;

namespace UP.Repositories;

public class AuthorizationRepository
{
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