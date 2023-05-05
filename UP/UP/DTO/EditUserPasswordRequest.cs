namespace UP.DTO;

public class EditUserPasswordRequest
{
    public int Id { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }


    public EditUserPasswordRequest(int id, string password, string passwordRepeat)
    {
        Id = id;
        Password = password;
        PasswordRepeat = passwordRepeat;
    }
}