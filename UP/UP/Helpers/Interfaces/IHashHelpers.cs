namespace Api.OpenAI.Handlers.Interfaces;

public interface IHashHelpers
{
    public string GenerateSalt(int size);
    public string HashPassword(string password, string salt);
}