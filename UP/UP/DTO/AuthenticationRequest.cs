namespace UP.DTO;

public class AuthenticationRequest
{
    public string Login { get; set; }
    public string Password { get; set; }

    public AuthenticationRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }
}