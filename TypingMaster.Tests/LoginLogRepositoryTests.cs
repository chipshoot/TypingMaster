using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests
{
    public class LoginLogRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly LoginLogRepository _repository;

        public LoginLogRepositoryTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"LoginLogTestDb_{Guid.NewGuid()}")
                .Options;

            // Create the in-memory database context
            _context = new ApplicationDbContext(options);

            // Setup test data
            SeedTestData();

            // Create repository with context
            _repository = new LoginLogRepository(_context);
        }

        private void SeedTestData()
        {
            var now = DateTime.UtcNow;

            // Add test login logs
            _context.LoginLogs.AddRange(
                new LoginLogDao
                {
                    Id = 1,
                    AccountId = 1,
                    IpAddress = "192.168.1.1",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                    LoginTime = now.AddDays(-1),
                    IsSuccessful = true,
                    FailureReason = null
                },
                new LoginLogDao
                {
                    Id = 2,
                    AccountId = 1,
                    IpAddress = "192.168.1.2",
                    UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)",
                    LoginTime = now.AddDays(-2),
                    IsSuccessful = true,
                    FailureReason = null
                },
                new LoginLogDao
                {
                    Id = 3,
                    AccountId = 2,
                    IpAddress = "10.0.0.1",
                    UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0)",
                    LoginTime = now.AddDays(-3),
                    IsSuccessful = false,
                    FailureReason = "Invalid credentials"
                },
                new LoginLogDao
                {
                    Id = 4,
                    AccountId = 3,
                    IpAddress = "10.0.0.2",
                    UserAgent = "Mozilla/5.0 (Android 10)",
                    LoginTime = now,
                    IsSuccessful = true,
                    FailureReason = null
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
        public async Task GetLoginLogsByAccountIdAsync_ReturnsLogsForSpecificAccount()
        {
            // Arrange
            var accountId = 1;

            // Act
            var result = await _repository.GetLoginLogsByAccountIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, log => Assert.Equal(accountId, log.AccountId));

            // Verify logs are ordered by time (descending)
            var orderedLogs = result.OrderByDescending(l => l.LoginTime).ToList();
            for (int i = 0; i < orderedLogs.Count; i++)
            {
                Assert.Equal(orderedLogs[i].Id, result.ElementAt(i).Id);
            }
        }

        [Fact]
        public async Task GetLoginLogsByAccountIdAsync_ReturnsEmptyCollection_WhenNoLogsExist()
        {
            // Arrange
            var accountId = 999; // Non-existent account ID

            // Act
            var result = await _repository.GetLoginLogsByAccountIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateLoginLogAsync_AddsLogToDatabase()
        {
            // Arrange
            var newLoginLog = new LoginLogDao
            {
                AccountId = 2,
                IpAddress = "172.16.0.1",
                UserAgent = "Mozilla/5.0 (Linux; Android 11)",
                LoginTime = DateTime.UtcNow,
                IsSuccessful = true,
                FailureReason = null
            };

            // Act
            var result = await _repository.CreateLoginLogAsync(newLoginLog);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newLoginLog.AccountId, result.AccountId);
            Assert.Equal(newLoginLog.IpAddress, result.IpAddress);
            Assert.Equal(newLoginLog.UserAgent, result.UserAgent);
            Assert.Equal(newLoginLog.LoginTime, result.LoginTime);
            Assert.Equal(newLoginLog.IsSuccessful, result.IsSuccessful);
            Assert.Equal(newLoginLog.FailureReason, result.FailureReason);

            // Verify the log was added to the database
            var savedLog = await _context.LoginLogs
                .FirstOrDefaultAsync(log => log.IpAddress == "172.16.0.1");
            Assert.NotNull(savedLog);
            Assert.Equal(newLoginLog.AccountId, savedLog.AccountId);
            Assert.Equal(newLoginLog.UserAgent, savedLog.UserAgent);
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_ReturnsSpecifiedNumberOfLogs()
        {
            // Arrange
            var count = 2;

            // Act
            var result = await _repository.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.Count());

            // Verify logs are ordered by time (newest first)
            var firstLog = result.First();
            var secondLog = result.Skip(1).First();
            Assert.True(firstLog.LoginTime >= secondLog.LoginTime);
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_ReturnsAllLogs_WhenCountIsLargerThanAvailable()
        {
            // Arrange
            var count = 10; // More than the 4 logs we have in the test data

            // Act
            var result = await _repository.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count()); // Should return all 4 available logs
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_ReturnsEmptyCollection_WhenDatabaseIsEmpty()
        {
            // Arrange
            _context.LoginLogs.RemoveRange(_context.LoginLogs);
            await _context.SaveChangesAsync();
            var count = 5;

            // Act
            var result = await _repository.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}