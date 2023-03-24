namespace UP.Models;

public class PreviosPasswords
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Password { get; set; }

    public PreviosPasswords(int id, int userId, string password)
    {
        Id = id;
        UserId = userId;
        Password = password;
    }

    public PreviosPasswords()
    {
        
    }
}