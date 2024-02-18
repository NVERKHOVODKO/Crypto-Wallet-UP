using Microsoft.EntityFrameworkCore;
using UP.Models;
using UP.ModelsEF;

namespace TestApplication.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
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
    }
}