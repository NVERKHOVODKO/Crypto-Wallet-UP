namespace Up.Core.DTO;

public class EditUserPasswordRequest
{
    public Guid Id { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }
}