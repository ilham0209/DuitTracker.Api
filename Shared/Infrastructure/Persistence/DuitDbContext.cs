using DuitTracker.Api.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Shared.Infrastructure.Persistence;

public class DuitDbContext(DbContextOptions<DuitDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Icon).IsRequired().HasMaxLength(50);
            e.Property(x => x.Color).IsRequired().HasMaxLength(20);
            e.Property(x => x.Type).IsRequired().HasMaxLength(20);
            e.Property(x => x.SysUserCreated).HasMaxLength(100);
            e.Property(x => x.SysUserModified).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);

            e.HasMany(x => x.Transactions)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(x => x.Budgets)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.CategoryId).IsRequired();
            e.Property(x => x.PaymentMethodId).IsRequired();
            e.Property(x => x.Amount).IsRequired().HasColumnType("numeric(18,2)");
            e.Property(x => x.Note).HasMaxLength(500);
            e.Property(x => x.TransactionDate).IsRequired();
            e.Property(x => x.ReferenceNo).HasMaxLength(100);
            e.Property(x => x.AttachmentUrl).HasMaxLength(500);
            e.Property(x => x.SysUserCreated).HasMaxLength(100);
            e.Property(x => x.SysUserModified).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);

            e.HasOne(x => x.PaymentMethod)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentMethod>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.SysUserCreated).HasMaxLength(100);
            e.Property(x => x.SysUserModified).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.CategoryId).IsRequired();
            e.Property(x => x.Amount).IsRequired().HasColumnType("numeric(18,2)");
            e.Property(x => x.Month).IsRequired();
            e.Property(x => x.Year).IsRequired();
            e.Property(x => x.SysUserCreated).HasMaxLength(100);
            e.Property(x => x.SysUserModified).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
            e.Property(x => x.Role).IsRequired().HasMaxLength(20);
            e.Property(x => x.SysUserCreated).HasMaxLength(100);
            e.Property(x => x.SysUserModified).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}