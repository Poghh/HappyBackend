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
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .IsRequired();
            entity.Property(e => e.TotalItems)
                .HasDefaultValue(0);
            entity.Property(e => e.ProcessedItems)
                .HasDefaultValue(0);
            entity.Property(e => e.ErrorDetails)
                .HasColumnType("jsonb");
            entity.Property(e => e.ProcessedAt);
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
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .IsRequired();
            entity.Property(e => e.TotalItems)
                .HasDefaultValue(0);
            entity.Property(e => e.ProcessedItems)
                .HasDefaultValue(0);
            entity.Property(e => e.ErrorDetails)
                .HasColumnType("jsonb");
            entity.Property(e => e.ProcessedAt);
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

        modelBuilder.Entity<StoreInformation>(entity =>
        {
            entity.ToTable("store_information");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppCredentialId)
                .IsRequired();
            entity.Property(e => e.StoreName)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsRequired();
            entity.HasOne(e => e.AppCredential)
                .WithMany()
                .HasForeignKey(e => e.AppCredentialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.AppCredentialId);
        });

        modelBuilder.Entity<FarmerInformation>(entity =>
        {
            entity.ToTable("farmer_information");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppCredentialId)
                .IsRequired();
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsRequired();
            entity.HasOne(e => e.AppCredential)
                .WithMany()
                .HasForeignKey(e => e.AppCredentialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.AppCredentialId);
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.ToTable("seasons");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SeasonName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.FarmingType).HasMaxLength(100);
            entity.Property(e => e.TargetYield);
            entity.Property(e => e.YieldUnit).HasMaxLength(50);
            entity.Property(e => e.ExpectedProfit).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ScaleValue);
            entity.Property(e => e.ScaleUnit).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasMaxLength(20);
            entity.Property(e => e.EndDate).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => e.Phone);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SeasonId).IsRequired();
            entity.Property(e => e.SeasonName).HasMaxLength(200);
            entity.Property(e => e.ExpenName).HasMaxLength(200);
            entity.Property(e => e.Date).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.IsDebt);
            entity.Property(e => e.ExpenPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImageUrls).HasColumnType("jsonb");
            entity.Property(e => e.ImageCount);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasOne(e => e.Season)
                .WithMany(s => s.Expenses)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.SeasonId);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("activities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SeasonId).IsRequired();
            entity.Property(e => e.SeasonName).HasMaxLength(200);
            entity.Property(e => e.ActivityName).HasMaxLength(200);
            entity.Property(e => e.DateTime).HasMaxLength(50);
            entity.Property(e => e.StatusLabel).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ImageUrls).HasColumnType("jsonb");
            entity.Property(e => e.ImageCount);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasOne(e => e.Season)
                .WithMany(s => s.Activities)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.SeasonId);
        });

        modelBuilder.Entity<Harvest>(entity =>
        {
            entity.ToTable("harvests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SeasonId).IsRequired();
            entity.Property(e => e.HarvestName).HasMaxLength(200);
            entity.Property(e => e.HarvestDate).HasMaxLength(50);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.QuantityUnit).HasMaxLength(50);
            entity.Property(e => e.Product).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Buyer).HasMaxLength(200);
            entity.Property(e => e.RecordDebt);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasOne(e => e.Season)
                .WithMany(s => s.Harvests)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.SeasonId);
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.ToTable("stock_ins");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ProductCode).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Platform).HasMaxLength(100);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.StockInDate).HasMaxLength(20);
            entity.Property(e => e.OfflineTempCode).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => new { e.ProductCode, e.Phone });
        });
    }

    public DbSet<SyncRawStore> SyncRawStores => Set<SyncRawStore>();
    public DbSet<SyncRawFarmer> SyncRawFarmers => Set<SyncRawFarmer>();
    public DbSet<AppCredential> AppCredentials => Set<AppCredential>();
    public DbSet<StoreInformation> StoreInformation => Set<StoreInformation>();
    public DbSet<FarmerInformation> FarmerInformation => Set<FarmerInformation>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<StockIn> StockIns => Set<StockIn>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Harvest> Harvests => Set<Harvest>();
}
