namespace UP.DTO;

public class EditUserLoginRequest
{
    public int Id { get; set; }
    public string Login { get; set; }

    public EditUserLoginRequest(int id, string login)
    {
        Id = id;
        Login = login;
    }
}