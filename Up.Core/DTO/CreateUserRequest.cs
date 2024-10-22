namespace Up.Core.DTO;

public class CreateUserRequest
{
    public Guid Id { get; set; }

    public string Nickname { get; set; }

    public string Password { get; set; }

    public string IPAddress { get; set; }

    public Guid ApplicationId { get; set; }
}