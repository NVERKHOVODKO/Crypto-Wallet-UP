namespace Up.Core.DTO;

public class EditUserRequest
{
    public EditUserRequest(int id, string login, string password, string passwordRepeat, string email)
    {
        Id = id;
        Login = login;
        Password = password;
        PasswordRepeat = passwordRepeat;
        Email = email;
    }

    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }
    public string Email { get; set; }
}