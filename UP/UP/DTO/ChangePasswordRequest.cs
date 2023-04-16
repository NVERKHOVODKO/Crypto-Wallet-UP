namespace UP.DTO;

public class ChangePasswordRequest
{
    public int Id { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }

    public ChangePasswordRequest(int id, string password, string passwordRepeat)
    {
        Id = id;
        Password = password;
        PasswordRepeat = passwordRepeat;
    }
}