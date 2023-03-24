namespace UP.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public String Ip { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}