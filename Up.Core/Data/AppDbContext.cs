namespace Up.Core.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        /*Database.EnsureDeleted();
        Database.EnsureCreated();*/
    }

    public DbSet<User> Users { get; set; }
    
    public DbSet<Blocking> Blockings { get; set; }
    public DbSet<Coin> Coins { get; set; }
    public DbSet<Conversion> Conversions { get; set; }
    
    public DbSet<LoginHistory> LoginHistories { get; set; }
    
    public DbSet<RestorePasswordCodeModel> RestorePasswordCodes { get; set; }
    
    public DbSet<PreviousPassword> PreviousPasswords { get; set; }
    
    public DbSet<EmailVerificationCodeModel> VerifyEmailCode { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    
    public DbSet<CryptoCurrencyPrices> CoinHistory { get; set; }
    public DbSet<UsersCoins> UsersCoins { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<Replenishment> Replenishments { get; set; }
    
    public DbSet<Service> Services { get; set; }
    
    public DbSet<CoinListInfo> CoinListInfos { get; set; }
    
    public DbSet<EmailVerificationCodeModel> EmailVerificationCodeModels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=up.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transactions>()
            .HasOne(t => t.Sender)
            .WithMany(u => u.SentTransactions)
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transactions>()
            .HasOne(t => t.Receiver)
            .WithMany(u => u.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
        
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));

            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
        }

        base.OnModelCreating(modelBuilder);
    }
    
    private static string ToSnakeCase(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return SnakeCaseRegex().Replace(Regex.Replace(input, @"([a-z])([A-Z])", "$1_$2"), "$1_$2")
            .ToLowerInvariant();
    }

    [GeneratedRegex(@"([A-Z])([A-Z][a-z])")]
    private static partial Regex SnakeCaseRegex();
}