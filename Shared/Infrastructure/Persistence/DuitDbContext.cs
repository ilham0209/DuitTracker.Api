using DuitTracker.Api.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Shared.Infrastructure.Persistence;

public class DuitDbContext(DbContextOptions<DuitDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Transaction>().HasQueryFilter(x => !x.IsDeleted);
    }
}