namespace TestApplication.DTO;

public class RestorePasswordRequest
{
    public string NewPassword { get; set; }
    public Guid UserId { get; set; }
}