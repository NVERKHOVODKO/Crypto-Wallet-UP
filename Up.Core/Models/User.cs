namespace Up.Core.Models;

public class User : BaseModel
{
    [Key] public Guid Id { get; set; }

    [Required] public string Login { get; set; }

    [Required] public string Password { get; set; }

    public string? Email { get; set; }

    [Required] public bool IsDeleted { get; set; }

    [Required] public int RoleId { get; set; }

    [Required] public bool IsBlocked { get; set; }

    [Required] public string Salt { get; set; }

    public virtual ICollection<Blocking> Blockings { get; set; }
    public virtual ICollection<Conversion> Conversions { get; set; }
    public virtual ICollection<UsersCoins> UsersCoins { get; set; }
    public virtual ICollection<LoginHistory> LoginHistories { get; set; }
    public virtual ICollection<PreviousPassword> PreviousPasswords { get; set; }
    public virtual ICollection<Replenishment> Replenishments { get; set; }
    public virtual ICollection<Transactions> SentTransactions { get; set; }
    public virtual ICollection<Transactions> ReceivedTransactions { get; set; }
    public virtual ICollection<Withdrawal> Withdrawals { get; set; }
}