using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;
using System.Text.RegularExpressions;

namespace TypingMaster.DataAccess.Data;


public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AccountDao> Accounts { get; set; }

    public DbSet<UserProfileDao> UserProfiles { get; set; }

    public DbSet<PracticeLogDao> PracticeLogs { get; set; }

    public DbSet<DrillStatsDao> DrillStats { get; set; }

    public DbSet<CourseDao> Courses { get; set; }

    public DbSet<LoginLogDao> LoginLogs { get; set; }

    public DbSet<LoginCredentialDao> LoginCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships and constraints
        modelBuilder.Entity<AccountDao>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column
            entity.HasIndex(e => e.AccountEmail).IsUnique();

            // Map camelCase property names to snake_case column names
            entity.Property(e => e.AccountEmail).HasColumnName("account_email");

            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<UserProfileDao>("AccountId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.History)
                .WithOne()
                .HasForeignKey<PracticeLogDao>("AccountId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with CourseDao
            entity.HasMany(e => e.Courses)
                .WithOne()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.GoalStats).HasColumnName("goal_stats").HasColumnType("jsonb").HasJsonConversion();
            entity.Property(e => e.Version)
                .HasColumnName("version")
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<PracticeLogDao>(entity =>
        {
            entity.ToTable("practices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column
            entity.Property(e => e.KeyStatsJson).HasColumnName("key_stats_json").HasColumnType("jsonb").HasJsonConversion();

            // Configure one-to-many relationship with DrillStatsDao
            entity.HasMany(p => p.PracticeStats)
                .WithOne()
                .HasForeignKey(d => d.PracticeLogId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // Configure DrillStatsDao relationship with PracticeLogDao
        modelBuilder.Entity<DrillStatsDao>(entity =>
        {
            entity.ToTable("drill_stats");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column

            entity.Property(e => e.KeyEventsJson).HasColumnName("key_events_json").HasColumnType("jsonb").HasJsonConversion();
            entity.Property(e => e.PracticeLogId).HasColumnName("practice_log_id");

            entity.HasKey(e => e.Id);

            entity.HasOne<PracticeLogDao>()
                .WithMany(p => p.PracticeStats)
                .HasForeignKey(e => e.PracticeLogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfileDao>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column
        });

        modelBuilder.Entity<CourseDao>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SettingsJson).HasColumnName("settings_json").HasColumnType("jsonb").HasJsonConversion();
            entity.Property(e => e.AccountId).HasColumnName("account_id");
        });

        modelBuilder.Entity<LoginLogDao>(entity =>
        {
            entity.ToTable("login_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column

            // Configure relationship with AccountDao
            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoginCredentialDao>(entity =>
        {
            entity.ToTable("login_credentials");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column

            // Create unique index on email
            entity.HasIndex(e => e.Email).IsUnique();

            // Configure relationship with AccountDao - one Account has one LoginCredential
            entity.HasOne(e => e.Account)
                .WithOne()
                .HasForeignKey<LoginCredentialDao>(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Map camelCase property names to snake_case column names
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.ExternalIdpId).HasColumnName("external_idp_id");
            entity.Property(e => e.ExternalIdpType).HasColumnName("external_idp_type");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.LastUpdated).HasColumnName("last_updated");
        });

        // Apply snake_case naming convention to all remaining properties
        ApplySnakeCaseNamingConvention(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplySnakeCaseNamingConvention(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Skip properties that already have a column name mapping
            foreach (var property in entity.GetProperties()
                     .Where(p => p.GetColumnName() == p.Name))
            {
                var columnName = ToSnakeCase(property.Name);
                property.SetColumnName(columnName);
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Convert camelCase to snake_case
        return Regex.Replace(
            input,
            @"([a-z0-9])([A-Z])",
            "$1_$2").ToLower();
    }
}