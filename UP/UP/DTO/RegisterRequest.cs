namespace UP.DTO;

public class RegisterRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }
    public string Email { get; set; }

    public RegisterRequest(string login, string password, string passwordRepeat, string email)
    {
        Login = login;
        Password = password;
        PasswordRepeat = passwordRepeat;
        Email = email;
    }
}