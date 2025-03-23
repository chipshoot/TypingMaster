using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.Tests
{
    public class AccountRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountRepository _repository;

        public AccountRepositoryTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"AccountTestDb_{Guid.NewGuid()}")
                .Options;

            // Create the in-memory database context
            _context = new ApplicationDbContext(options);

            // Setup test data
            SeedTestData();

            // Create logger mock
            var mockLogger = new Mock<ILogger>();

            // Create user profile repository mock
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(r => r.ProcessResult).Returns(new ProcessResult(mockLogger.Object));

            // Configure mock for UpdateUserProfileAsync
            mockUserProfileRepository
                .Setup(r => r.UpdateUserProfileAsync(It.IsAny<UserProfileDao>()))
                .ReturnsAsync((UserProfileDao userProfile) =>
                {
                    Console.WriteLine($"Mock UpdateUserProfileAsync called with id: {userProfile?.Id}");

                    if (userProfile == null)
                    {
                        Console.WriteLine("User profile is null");
                        return null;
                    }

                    // Check if user profile exists
                    var existingUserProfile = _context.UserProfiles
                        .FirstOrDefault(u => u.Id == userProfile.Id);

                    if (existingUserProfile == null)
                    {
                        Console.WriteLine($"User profile {userProfile.Id} not found in DB");
                        // Create it if not found (for testing purposes)
                        if (userProfile.Id == 0)
                        {
                            userProfile.Id = _context.UserProfiles.Any()
                                ? _context.UserProfiles.Max(u => u.Id) + 1
                                : 1;
                        }

                        _context.UserProfiles.Add(userProfile);
                        _context.SaveChanges();

                        Console.WriteLine($"Created new user profile with ID: {userProfile.Id}");
                        return userProfile;
                    }

                    // Update properties
                    Console.WriteLine($"Updating user profile {existingUserProfile.Id}");
                    existingUserProfile.FirstName = userProfile.FirstName;
                    existingUserProfile.LastName = userProfile.LastName;
                    existingUserProfile.Title = userProfile.Title;
                    existingUserProfile.PhoneNumber = userProfile.PhoneNumber;
                    existingUserProfile.AvatarUrl = userProfile.AvatarUrl;

                    _context.SaveChanges();

                    Console.WriteLine($"User profile {existingUserProfile.Id} updated successfully");
                    return existingUserProfile;
                });

            // Configure mock for CreateUserProfileAsync
            mockUserProfileRepository
                .Setup(r => r.CreateUserProfileAsync(It.IsAny<UserProfileDao>()))
                .ReturnsAsync((UserProfileDao userProfile) =>
                {
                    Console.WriteLine($"Mock CreateUserProfileAsync called");

                    // Add the user profile to the database
                    if (userProfile.Id == 0)
                    {
                        userProfile.Id = _context.UserProfiles.Any()
                            ? _context.UserProfiles.Max(u => u.Id) + 1
                            : 1;
                    }

                    // If not already in the database, add it
                    if (!_context.UserProfiles.Any(u => u.Id == userProfile.Id))
                    {
                        _context.UserProfiles.Add(userProfile);
                        _context.SaveChanges();
                        Console.WriteLine($"Created user profile with ID: {userProfile.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"User profile {userProfile.Id} already exists");
                    }

                    return userProfile;
                });

            // Create practice log repository mock
            var mockPracticeLogRepository = new Mock<IPracticeLogRepository>();
            mockPracticeLogRepository.Setup(r => r.ProcessResult).Returns(new ProcessResult(mockLogger.Object));

            // Configure mock for AddDrillStatAsync
            mockPracticeLogRepository
                .Setup(r => r.AddDrillStatAsync(It.IsAny<int>(), It.IsAny<DrillStatsDao>()))
                .ReturnsAsync((int practiceLogId, DrillStatsDao drillStat) =>
                {
                    Console.WriteLine($"Mock AddDrillStatAsync called with practiceLogId: {practiceLogId}");

                    // Find the practice log in the database
                    var practiceLog = _context.PracticeLogs
                        .Include(p => p.PracticeStats)
                        .FirstOrDefault(p => p.Id == practiceLogId);

                    if (practiceLog == null)
                    {
                        Console.WriteLine("Practice log not found");
                        return null;
                    }

                    // Add the drill stat directly to the database
                    drillStat.PracticeLogId = practiceLogId;
                    _context.DrillStats.Add(drillStat);
                    _context.SaveChanges();

                    Console.WriteLine($"Drill stat added to practice log {practiceLogId}");
                    return drillStat;
                });

            // Configure mock for GetPracticeLogByIdAsync
            mockPracticeLogRepository
                .Setup(r => r.GetPracticeLogByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    Console.WriteLine($"Mock GetPracticeLogByIdAsync called with id: {id}");

                    var practiceLog = _context.PracticeLogs
                        .Include(p => p.PracticeStats)
                        .FirstOrDefault(p => p.Id == id);

                    Console.WriteLine($"Practice log found: {(practiceLog != null ? "yes" : "no")}");
                    return practiceLog;
                });

            // Configure mock for CreatePracticeLogAsync
            mockPracticeLogRepository
                .Setup(r => r.CreatePracticeLogAsync(It.IsAny<PracticeLogDao>()))
                .ReturnsAsync((PracticeLogDao practiceLog) =>
                {
                    Console.WriteLine($"Mock CreatePracticeLogAsync called");

                    // Add the practice log to the database
                    if (practiceLog.Id == 0)
                    {
                        practiceLog.Id = _context.PracticeLogs.Any()
                            ? _context.PracticeLogs.Max(p => p.Id) + 1
                            : 1;
                    }

                    // If not already in the database, add it
                    if (!_context.PracticeLogs.Any(p => p.Id == practiceLog.Id))
                    {
                        _context.PracticeLogs.Add(practiceLog);
                        _context.SaveChanges();
                        Console.WriteLine($"Created practice log with ID: {practiceLog.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"Practice log {practiceLog.Id} already exists");
                    }

                    return practiceLog;
                });

            // Configure mock for UpdatePracticeLogAsync
            mockPracticeLogRepository
                .Setup(r => r.UpdatePracticeLogAsync(It.IsAny<PracticeLogDao>()))
                .ReturnsAsync((PracticeLogDao practiceLog) =>
                {
                    Console.WriteLine($"Mock UpdatePracticeLogAsync called with id: {practiceLog?.Id}");

                    if (practiceLog == null)
                    {
                        Console.WriteLine("Practice log is null");
                        return null;
                    }

                    // Check if practice log exists
                    var existingPracticeLog = _context.PracticeLogs
                        .Include(p => p.PracticeStats)
                        .FirstOrDefault(p => p.Id == practiceLog.Id);

                    if (existingPracticeLog == null)
                    {
                        Console.WriteLine($"Practice log {practiceLog.Id} not found in DB");
                        // Create it if not found (for testing purposes)
                        if (practiceLog.Id == 0)
                        {
                            practiceLog.Id = _context.PracticeLogs.Any()
                                ? _context.PracticeLogs.Max(p => p.Id) + 1
                                : 1;
                        }

                        _context.PracticeLogs.Add(practiceLog);
                        _context.SaveChanges();

                        Console.WriteLine($"Created new practice log with ID: {practiceLog.Id}");
                        return practiceLog;
                    }

                    // Update properties
                    Console.WriteLine($"Updating practice log {existingPracticeLog.Id}");
                    existingPracticeLog.CurrentCourseId = practiceLog.CurrentCourseId;
                    existingPracticeLog.CurrentLessonId = practiceLog.CurrentLessonId;
                    existingPracticeLog.KeyStatsJson = practiceLog.KeyStatsJson;

                    // Handle PracticeStats if present
                    if (practiceLog.PracticeStats != null && practiceLog.PracticeStats.Any())
                    {
                        foreach (var stat in practiceLog.PracticeStats)
                        {
                            if (stat.Id == 0)
                            {
                                // Add new stats
                                stat.PracticeLogId = existingPracticeLog.Id;
                                _context.DrillStats.Add(stat);
                            }
                            else
                            {
                                // Update existing stats
                                var existingStat = _context.DrillStats.Find(stat.Id);
                                if (existingStat != null)
                                {
                                    _context.Entry(existingStat).CurrentValues.SetValues(stat);
                                }
                            }
                        }
                    }

                    _context.SaveChanges();

                    Console.WriteLine($"Practice log {existingPracticeLog.Id} updated successfully");
                    return existingPracticeLog;
                });

            // Create repository with real context and mocks
            _repository = new AccountRepository(
                _context,
                mockUserProfileRepository.Object,
                mockPracticeLogRepository.Object,
                mockLogger.Object);

            Console.WriteLine("AccountRepositoryTests constructor completed");
        }

        private void SeedTestData()
        {
            // Add practice logs first to get assigned IDs
            var practiceLog1 = new PracticeLogDao
            {
                KeyStatsJson = new Dictionary<char, KeyStatsDao>
                {
                    { 'a', new KeyStatsDao { Key = "a", TypingCount = 50, CorrectCount = 45 } },
                    { 's', new KeyStatsDao { Key = "s", TypingCount = 40, CorrectCount = 38 } }
                }
            };

            var practiceLog2 = new PracticeLogDao
            {
                KeyStatsJson = new Dictionary<char, KeyStatsDao>
                {
                    { 'd', new KeyStatsDao { Key = "d", TypingCount = 45, CorrectCount = 40 } },
                    { 'f', new KeyStatsDao { Key = "f", TypingCount = 35, CorrectCount = 30 } }
                }
            };

            _context.PracticeLogs.AddRange(practiceLog1, practiceLog2);
            _context.SaveChanges();

            // Add test accounts
            _context.Accounts.AddRange(
                new AccountDao
                {
                    Id = 1,
                    AccountName = "Test1",
                    AccountEmail = "test1@example.com",
                    GoalStats = new StatsDao { Wpm = 80, Accuracy = 95 },
                    User = new UserProfileDao { FirstName = "Test", LastName = "User1" },
                    History = practiceLog1,
                    IsDeleted = false
                },
                new AccountDao
                {
                    Id = 2,
                    AccountName = "Test2",
                    AccountEmail = "test2@example.com",
                    GoalStats = new StatsDao { Wpm = 75, Accuracy = 90 },
                    User = new UserProfileDao { FirstName = "Test", LastName = "User2" },
                    History = practiceLog2,
                    IsDeleted = false
                },
                new AccountDao
                {
                    Id = 3,
                    AccountName = "Test3",
                    AccountEmail = "test3@example.com",
                    GoalStats = new StatsDao { Wpm = 60, Accuracy = 85 },
                    User = new UserProfileDao { FirstName = "Test", LastName = "User3" },
                    History = new PracticeLogDao
                    {
                        KeyStatsJson = new Dictionary<char, KeyStatsDao>
                        {
                            { 'j', new KeyStatsDao { Key = "j", TypingCount = 30, CorrectCount = 25 } },
                            { 'k', new KeyStatsDao { Key = "k", TypingCount = 25, CorrectCount = 20 } }
                        }
                    },
                    IsDeleted = true
                }
            );

            _context.SaveChanges();
        }

        public void Dispose()
        {
            // Clean up the database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAccountsAsync_ReturnsNonDeletedAccounts()
        {
            // Act
            var result = await _repository.GetAllAccountsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.DoesNotContain(result, a => a.IsDeleted);
        }

        [Fact]
        public async Task GetAccountByIdAsync_ReturnsAccount_WhenIdExists()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _repository.GetAccountByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Test1", result.AccountName);
        }

        [Fact]
        public async Task GetAccountByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _repository.GetAccountByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccountByIdAsync_ReturnsNull_WhenAccountIsDeleted()
        {
            // Arrange - account with ID 3 is marked as deleted in seed data
            var id = 3;

            // Act
            var result = await _repository.GetAccountByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ReturnsAccount_WhenEmailExists()
        {
            // Arrange
            var email = "test1@example.com";

            // Act
            var result = await _repository.GetAccountByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.AccountEmail);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var result = await _repository.GetAccountByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccountAsync_AddsAccountToDatabase()
        {
            // Arrange
            var newAccount = new AccountDao
            {
                AccountName = "NewAccount",
                AccountEmail = "new@example.com",
                GoalStats = new StatsDao { Wpm = 70, Accuracy = 92 },
                User = new UserProfileDao { FirstName = "New", LastName = "User" },
                History = new PracticeLogDao
                {
                    KeyStatsJson = new Dictionary<char, KeyStatsDao>
                    {
                        { 'e', new KeyStatsDao { Key = "e", TypingCount = 60, CorrectCount = 55 } },
                        { 'n', new KeyStatsDao { Key = "n", TypingCount = 50, CorrectCount = 46 } }
                    }
                }
            };

            // Act
            var result = await _repository.CreateAccountAsync(newAccount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAccount.AccountName, result.AccountName);

            // Verify the account was added to the database
            var savedAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountEmail == "new@example.com");
            Assert.NotNull(savedAccount);
            Assert.Equal(70, savedAccount.GoalStats.Wpm);
            Assert.Equal(92, savedAccount.GoalStats.Accuracy);
            Assert.NotNull(savedAccount.History.KeyStatsJson);
            Assert.Equal(2, savedAccount.History.KeyStatsJson.Count);
        }

        [Fact]
        public async Task UpdateAccountAsync_UpdatesExistingAccount()
        {
            // Arrange
            var account = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.Id == 1);

            Assert.NotNull(account);
            Assert.NotNull(account.History); // Check if history exists

            account.AccountName = "Updated";

            // Act
            var result = await _repository.UpdateAccountAsync(account);

            // Debug information
            if (result == null)
            {
                // Check if the account still exists in DB
                var dbAccount = await _context.Accounts
                    .Include(a => a.User)
                    .Include(a => a.History)
                    .FirstOrDefaultAsync(a => a.Id == 1);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.AccountName);

            // Verify the change was saved to the database
            var updatedAccount = await _context.Accounts.FindAsync(1);
            Assert.NotNull(updatedAccount);
            Assert.Equal("Updated", updatedAccount.AccountName);
        }

        [Fact]
        public async Task DeleteAccountAsync_ReturnsFalse_WhenAccountDoesNotExist()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _repository.DeleteAccountAsync(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAccountAsync_RemovesAccount_WhenAccountExists()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _repository.DeleteAccountAsync(id);

            // Assert
            Assert.True(result);

            // Verify the account was removed from the database
            var deletedAccount = await _context.Accounts.FindAsync(id);
            Assert.Null(deletedAccount);
        }

        [Fact]
        public async Task AddDrillStatToAccountHistoryAsync_ValidAccountWithHistory_AddsStatAndReturnsUpdatedAccount()
        {
            // Arrange
            var practiceLog = new PracticeLogDao
            {
                CurrentCourseId = Guid.NewGuid(),
                CurrentLessonId = 1,
                KeyStatsJson = new Dictionary<char, KeyStatsDao>(),
                PracticeStats = new List<DrillStatsDao>()
            };

            var account = new AccountDao
            {
                AccountName = "test_user",
                AccountEmail = "test@example.com",
                GoalStats = new StatsDao(),
                History = practiceLog
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var drillStat = new DrillStatsDao
            {
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Accuracy = 95.5,
                Wpm = 60,
                TypedText = "test text",
                KeyEventsJson = new Queue<KeyEventDao>()
            };

            // Act
            var result = await _repository.AddDrillStatToAccountHistoryAsync(account.Id, drillStat);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.History);
            Assert.NotNull(result.History.PracticeStats);
            Assert.Contains(result.History.PracticeStats, ds => ds.Accuracy == 95.5 && ds.Wpm == 60);

            // Verify it was added to the database
            var fromDb = await _context.Accounts
                .Include(a => a.History)
                .ThenInclude(h => h.PracticeStats)
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            Assert.NotNull(fromDb);
            Assert.NotNull(fromDb.History);
            Assert.NotNull(fromDb.History.PracticeStats);
            Assert.Contains(fromDb.History.PracticeStats, ds => ds.Accuracy == 95.5 && ds.Wpm == 60);
        }

        [Fact]
        public async Task AddDrillStatToAccountHistoryAsync_NonExistingAccount_ReturnsNull()
        {
            // Arrange
            var drillStat = new DrillStatsDao
            {
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Accuracy = 95.5,
                Wpm = 60,
                TypedText = "test text",
                KeyEventsJson = new Queue<KeyEventDao>()
            };

            // Act
            var result = await _repository.AddDrillStatToAccountHistoryAsync(999, drillStat);

            // Assert
            Assert.Null(result);
            Assert.True(_repository.ProcessResult.HasErrors);
        }

        [Fact]
        public async Task AddDrillStatToAccountHistoryAsync_AccountWithoutHistory_ReturnsNull()
        {
            // Arrange
            var account = new AccountDao
            {
                AccountName = "test_user",
                AccountEmail = "test@example.com",
                GoalStats = new StatsDao(),
                History = null
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var drillStat = new DrillStatsDao
            {
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Accuracy = 95.5,
                Wpm = 60,
                TypedText = "test text",
                KeyEventsJson = new Queue<KeyEventDao>()
            };

            // Act
            var result = await _repository.AddDrillStatToAccountHistoryAsync(account.Id, drillStat);

            // Assert
            Assert.Null(result);
            Assert.True(_repository.ProcessResult.HasErrors);
        }
    }
}