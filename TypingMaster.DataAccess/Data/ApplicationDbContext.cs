using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;


public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AccountDao> Accounts { get; set; }

    public DbSet<UserProfileDao> UserProfiles { get; set; }

    public DbSet<PracticeLogDao> PracticeLogs { get; set; }

    public DbSet<DrillStatsDao> DrillStats { get; set; }

    public DbSet<CourseDao> Courses { get; set; }

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

            entity.Property(e => e.GoalStats).HasColumnType("jsonb").HasJsonConversion();
            entity.Property(e => e.Version)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<PracticeLogDao>(entity =>
        {
            entity.ToTable("practices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .UseIdentityByDefaultColumn(); // PostgresSQL-specific identity column
            entity.Property(e => e.KeyStatsJson).HasColumnType("jsonb").HasJsonConversion();

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

            entity.Property(e => e.KeyEventsJson).HasColumnType("jsonb").HasJsonConversion();

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

            entity.Property(e => e.SettingsJson).HasColumnType("jsonb").HasJsonConversion();
        });

        base.OnModelCreating(modelBuilder);
    }
}