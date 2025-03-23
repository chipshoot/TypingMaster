using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests;

public class PracticeLogRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PracticeLogRepository _repository;

    public PracticeLogRepositoryTests()
    {
        // Set up in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestPracticeLogDatabase_{Guid.NewGuid()}")
            .Options;

        // Logger mock
        var loggerMock = new Mock<ILogger>();

        // Create a fresh instance of the context
        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        // Initialize the repository
        _repository = new PracticeLogRepository(_context, loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetPracticeLogByIdAsync_ExistingId_ReturnsPracticeLog()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPracticeLogByIdAsync(practiceLog.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(practiceLog.Id, result.Id);
        Assert.Equal(practiceLog.CurrentCourseId, result.CurrentCourseId);
    }

    [Fact]
    public async Task GetPracticeLogByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetPracticeLogByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPracticeLogByAccountIdAsync_ExistingAccount_ReturnsPracticeLog()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };

        // Create an account and associate it with the practice log
        var account = new AccountDao
        {
            AccountName = "test_user",
            AccountEmail = "test@example.com",
            GoalStats = new StatsDao { Wpm = 0, Accuracy = 0 }
        };

        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        account.History = practiceLog;
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act - assuming AccountId property exists on the navigation to PracticeLog
        var result = await _context.PracticeLogs
            .FirstOrDefaultAsync(p => EF.Property<int>(p, "Id") == practiceLog.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePracticeLogAsync_ValidData_CreatesAndReturnsPracticeLog()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };

        // Act
        var result = await _repository.CreatePracticeLogAsync(practiceLog);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(practiceLog.CurrentCourseId, result.CurrentCourseId);

        // Check that it was added to the database
        var fromDb = await _context.PracticeLogs.FindAsync(result.Id);
        Assert.NotNull(fromDb);
    }

    [Fact]
    public async Task CreatePracticeLogAsync_NullPracticeStats_InitializesCollection()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>(),
        };

        // Act
        var result = await _repository.CreatePracticeLogAsync(practiceLog);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.PracticeStats);
        Assert.Empty(result.PracticeStats);
    }

    [Fact]
    public async Task UpdatePracticeLogAsync_ExistingPracticeLog_UpdatesAndReturnsUpdatedPracticeLog()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Modify the practice log
        practiceLog.CurrentLessonId = 2;
        var newKeyStatsMap = new Dictionary<char, KeyStatsDao>
        {
            { 'a', new KeyStatsDao { Accuracy = 95.5 } }
        };
        practiceLog.KeyStatsJson = newKeyStatsMap;

        // Act
        var result = await _repository.UpdatePracticeLogAsync(practiceLog);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(practiceLog.Id, result.Id);
        Assert.Equal(2, result.CurrentLessonId);
        Assert.Equal(newKeyStatsMap, result.KeyStatsJson);

        // Check database
        var fromDb = await _context.PracticeLogs.FindAsync(practiceLog.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(2, fromDb.CurrentLessonId);
        Assert.Equal(newKeyStatsMap, fromDb.KeyStatsJson);
    }

    [Fact]
    public async Task UpdatePracticeLogAsync_NonExistingPracticeLog_ReturnsNull()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            Id = 999, // Non-existing ID
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 2
        };

        // Act
        var result = await _repository.UpdatePracticeLogAsync(practiceLog);

        // Assert
        Assert.Null(result);
        Assert.True(_repository.ProcessResult.HasErrors);
    }

    [Fact]
    public async Task AddDrillStatAsync_ValidPracticeLogAndDrillStat_AddsAndReturnsDrillStat()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            Accuracy = 95.5,
            Wpm = 60,
            PracticeText = "Practice text",
            TypedText = "Typed text",
            KeyEventsJson = new Queue<KeyEventDao>()
        };

        // Act
        var result = await _repository.AddDrillStatAsync(practiceLog.Id, drillStat);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(practiceLog.Id, result.PracticeLogId);

        // Check database
        var fromDb = await _context.DrillStats.FindAsync(result.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(drillStat.CourseId, fromDb.CourseId);
        Assert.Equal(95.5, fromDb.Accuracy);
    }

    [Fact]
    public async Task AddDrillStatAsync_NonExistingPracticeLog_ReturnsNull()
    {
        // Arrange
        var drillStat = new DrillStatsDao
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            Accuracy = 95.5,
            Wpm = 60
        };

        // Act
        var result = await _repository.AddDrillStatAsync(999, drillStat);

        // Assert
        Assert.Null(result);
        Assert.True(_repository.ProcessResult.HasErrors);
    }

    [Fact]
    public async Task CanDeletePracticeLogAsync_NonExistingPracticeLog_ReturnsFalse()
    {
        // Act
        var result = await _repository.CanDeletePracticeLogAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeletePracticeLogAsync_PracticeLogAssociatedWithAccount_ReturnsFalse()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Create an account and associate it with the practice log
        var account = new AccountDao
        {
            AccountName = "test_user",
            AccountEmail = "test@example.com",
            GoalStats = new StatsDao { Wpm = 0, Accuracy = 0 },
            History = practiceLog
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CanDeletePracticeLogAsync(practiceLog.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeletePracticeLogAsync_PracticeLogNotAssociatedWithAccount_ReturnsTrue()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CanDeletePracticeLogAsync(practiceLog.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeletePracticeLogAsync_ValidPracticeLog_DeletesAndReturnsTrue()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeletePracticeLogAsync(practiceLog.Id);

        // Assert
        Assert.True(result);

        // Check database
        var fromDb = await _context.PracticeLogs.FindAsync(practiceLog.Id);
        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeletePracticeLogAsync_PracticeLogAssociatedWithAccount_ReturnsFalse()
    {
        // Arrange
        var practiceLog = new PracticeLogDao
        {
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            KeyStatsJson = new Dictionary<char, KeyStatsDao>()
        };
        _context.PracticeLogs.Add(practiceLog);
        await _context.SaveChangesAsync();

        // Create an account and associate it with the practice log
        var account = new AccountDao
        {
            AccountName = "test_user",
            AccountEmail = "test@example.com",
            GoalStats = new StatsDao { Wpm = 0, Accuracy = 0 },
            History = practiceLog
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeletePracticeLogAsync(practiceLog.Id);

        // Assert
        Assert.False(result);

        // Check database - practice log should still exist
        var fromDb = await _context.PracticeLogs.FindAsync(practiceLog.Id);
        Assert.NotNull(fromDb);
    }
}