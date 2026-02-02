using Happy.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Infrastructure.Data;

public class HappyDbContext : DbContext
{
    public HappyDbContext(DbContextOptions<HappyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SyncRawStore>(entity =>
        {
            entity.ToTable("sync_raw_store");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(e => e.SyncTime)
                .IsRequired();
            entity.Property(e => e.SyncData)
                .HasColumnType("jsonb")
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<SyncRawFarmer>(entity =>
        {
            entity.ToTable("sync_raw_farmer");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(e => e.SyncTime)
                .IsRequired();
            entity.Property(e => e.SyncData)
                .HasColumnType("jsonb")
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<AppCredential>(entity =>
        {
            entity.ToTable("app_credentials");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppSecret)
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(e => e.AppName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
            entity.HasIndex(e => e.AppSecret)
                .IsUnique();
        });
    }

    public DbSet<SyncRawStore> SyncRawStores => Set<SyncRawStore>();
    public DbSet<SyncRawFarmer> SyncRawFarmers => Set<SyncRawFarmer>();
    public DbSet<AppCredential> AppCredentials => Set<AppCredential>();
}