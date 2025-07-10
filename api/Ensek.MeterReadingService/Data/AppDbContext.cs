using Ensek.MeterReadingService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.MeterReadingService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountId)
            .ValueGeneratedNever();
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
}
