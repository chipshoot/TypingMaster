using Microsoft.EntityFrameworkCore;
using Serilog;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;
using Xunit;

namespace TypingMaster.Tests;

public class DrillStatsRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DrillStatsRepository _repository;
    private readonly Serilog.ILogger _logger;

    public DrillStatsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _logger = new LoggerConfiguration().CreateLogger();
        _repository = new DrillStatsRepository(_context, _logger);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetDrillStatByIdAsync_ExistingDrillStat_ReturnsDrillStat()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            PracticeText = "Test text",
            TypedText = "Typed text",
            Accuracy = 95.5,
            Wpm = 60,
            KeyEventsJson = new Queue<KeyEventDao>()
        };
        _context.DrillStats.Add(drillStat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDrillStatByIdAsync(drillStat.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(drillStat.CourseId, result.CourseId);
        Assert.Equal(drillStat.Accuracy, result.Accuracy);
        Assert.Equal(drillStat.Wpm, result.Wpm);
    }

    [Fact]
    public async Task GetDrillStatByIdAsync_NonExistingDrillStat_ReturnsNull()
    {
        // Act
        var result = await _repository.GetDrillStatByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDrillStatsByPracticeLogIdAsync_ExistingPracticeLog_ReturnsDrillStats()
    {
        // Arrange
        var practiceLogId = 1;
        var drillStats = new List<DrillStatsDao>
        {
            new()
            {
                PracticeLogId = practiceLogId,
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Accuracy = 95.5,
                Wpm = 60
            },
            new()
            {
                PracticeLogId = practiceLogId,
                CourseId = Guid.NewGuid(),
                LessonId = 2,
                Accuracy = 92.0,
                Wpm = 65
            }
        };

        _context.DrillStats.AddRange(drillStats);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPaginatedDrillStatsByPracticeLogIdAsync(practiceLogId);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, ds => Assert.Equal(practiceLogId, ds.PracticeLogId));
    }

    [Fact]
    public async Task CreateDrillStatAsync_ValidDrillStat_CreatesDrillStat()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            PracticeText = "Test text",
            TypedText = "Typed text",
            Accuracy = 95.5,
            Wpm = 60
        };

        // Act
        var result = await _repository.CreateDrillStatAsync(drillStat);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.NotNull(result.KeyEventsJson);

        // Verify in database
        var fromDb = await _context.DrillStats.FindAsync(result.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(drillStat.CourseId, fromDb.CourseId);
        Assert.Equal(drillStat.Accuracy, fromDb.Accuracy);
    }

    [Fact]
    public async Task UpdateDrillStatAsync_ExistingDrillStat_UpdatesDrillStat()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            PracticeText = "Test text",
            TypedText = "Typed text",
            Accuracy = 95.5,
            Wpm = 60,
            KeyEventsJson = new Queue<KeyEventDao>()
        };
        _context.DrillStats.Add(drillStat);
        await _context.SaveChangesAsync();

        // Update values
        drillStat.Accuracy = 98.0;
        drillStat.Wpm = 70;

        // Act
        var result = await _repository.UpdateDrillStatAsync(drillStat);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(98.0, result.Accuracy);
        Assert.Equal(70, result.Wpm);

        // Verify in database
        var fromDb = await _context.DrillStats.FindAsync(drillStat.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(98.0, fromDb.Accuracy);
        Assert.Equal(70, fromDb.Wpm);
    }

    [Fact]
    public async Task UpdateDrillStatAsync_NonExistingDrillStat_ReturnsNull()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            Id = 999,
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            Accuracy = 95.5,
            Wpm = 60
        };

        // Act
        var result = await _repository.UpdateDrillStatAsync(drillStat);

        // Assert
        Assert.Null(result);
        Assert.True(_repository.ProcessResult.HasErrors);
    }

    [Fact]
    public async Task DeleteDrillStatAsync_ExistingDrillStat_DeletesDrillStat()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            PracticeText = "Test text",
            TypedText = "Typed text",
            Accuracy = 95.5,
            Wpm = 60
        };
        _context.DrillStats.Add(drillStat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteDrillStatAsync(drillStat.Id);

        // Assert
        Assert.True(result);
        var fromDb = await _context.DrillStats.FindAsync(drillStat.Id);
        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeleteDrillStatAsync_NonExistingDrillStat_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteDrillStatAsync(999);

        // Assert
        Assert.False(result);
        Assert.True(_repository.ProcessResult.HasErrors);
    }
}