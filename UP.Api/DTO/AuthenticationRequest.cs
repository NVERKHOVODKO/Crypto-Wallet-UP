namespace UP.DTO;

public class AuthenticationRequest
{
    public AuthenticationRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; set; }
    public string Password { get; set; }
}