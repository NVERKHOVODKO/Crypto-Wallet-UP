namespace Up.Core.DTO;

public class ChangePasswordRequest
{
    public Guid Id { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }
}