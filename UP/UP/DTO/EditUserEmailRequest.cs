namespace UP.DTO;

public class EditUserEmailRequest
{
    public int Id { get; set; }
    public string Email { get; set; }

    public EditUserEmailRequest(int id, string email)
    {
        Id = id;
        Email = email;
    }
}