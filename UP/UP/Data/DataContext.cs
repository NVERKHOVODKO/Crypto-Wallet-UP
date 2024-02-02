using Microsoft.EntityFrameworkCore;

namespace TestApplication.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        /*Database.EnsureDeleted();
        Database.EnsureCreated();*/
    }

    /*public DbSet<UserModel> Users { get; set; }
    public DbSet<RoleModel> Roles { get; set; }
    public DbSet<UserRoleModel> UserRoles { get; set; }
    public DbSet<LoginHistoryModel> LoginHistory { get; set; }
    public DbSet<ChatModel> Chats { get; set; }
    public DbSet<RestorePasswordCodeModel> RestorePasswordCodes { get; set; }
    public DbSet<MessageModel> Messages { get; set; }
    public DbSet<EmailVerificationCodeModel> VerifyEmailCode { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoleModel>()
            .HasOne(u => u.UserModel)
            .WithMany(ur => ur.UserRoleModels)
            .HasForeignKey(ui => ui.UserId);

        modelBuilder.Entity<UserRoleModel>()
            .HasOne(r => r.RoleModel)
            .WithMany(ur => ur.UserRoleModels)
            .HasForeignKey(ri => ri.RoleId);

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Id)
            .IsUnique();
    }*/
}