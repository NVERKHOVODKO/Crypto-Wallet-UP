namespace UP.Models
{
    public class User
    {
        public int Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public DateTime CreationData { get; set; }
        public DateTime ModificationDate { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsBlocked { get; set; }
        public int RoleId { get; set; }
        public string Salt { get; set; }


        public User(int id, String login, String password, String email, DateTime creationData,
            DateTime modificationDate, Boolean isDeleted, Boolean isBlocked, int roleId, string salt)
        {
            Id = id;
            Login = login;
            Password = password;
            Email = email;
            CreationData = creationData;
            ModificationDate = modificationDate;
            IsDeleted = isDeleted;
            IsBlocked = isBlocked;
            RoleId = roleId;
            Salt = salt;
        }
    }
}