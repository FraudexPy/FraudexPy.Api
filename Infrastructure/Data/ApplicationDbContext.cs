using FraudexPy.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudexPy.Api.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FraudReport> FraudReports => Set<FraudReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(150);
            entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<FraudReport>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Title).HasMaxLength(200).IsRequired();
            entity.Property(f => f.SuspectIdentity).HasMaxLength(200);
            entity.Property(f => f.EstimatedLoss).HasPrecision(18, 2);
            entity.Property(f => f.Currency).HasMaxLength(10).HasDefaultValue("USD");
            entity.Property(f => f.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(f => f.RiskLevel).HasMaxLength(50).HasDefaultValue("Medium");

            entity.HasOne(f => f.ReportedByUser)
                .WithMany(u => u.FraudReports)
                .HasForeignKey(f => f.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
