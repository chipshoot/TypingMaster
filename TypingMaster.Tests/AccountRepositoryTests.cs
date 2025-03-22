using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

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

            // Create course service mock
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();

            // Create repository with real context and mock logger
            _repository = new AccountRepository(_context, mockUserProfileRepository.Object, mockLogger.Object);
        }

        private void SeedTestData()
        {
            // Add test accounts
            _context.Accounts.AddRange(
                new AccountDao
                {
                    Id = 1,
                    AccountName = "Test1",
                    AccountEmail = "test1@example.com",
                    GoalStats = new StatsDao { Wpm = 80, Accuracy = 95 },
                    User = new UserProfileDao { FirstName = "Test", LastName = "User1" },
                    History = new PracticeLogDao
                    {
                        KeyStatsJson = new Dictionary<char, KeyStatsDao>
                        {
                            { 'a', new KeyStatsDao { Key = "a", TypingCount = 50, CorrectCount = 45 } },
                            { 's', new KeyStatsDao { Key = "s", TypingCount = 40, CorrectCount = 38 } }
                        }
                    },
                    IsDeleted = false
                },
                new AccountDao
                {
                    Id = 2,
                    AccountName = "Test2",
                    AccountEmail = "test2@example.com",
                    GoalStats = new StatsDao { Wpm = 75, Accuracy = 90 },
                    User = new UserProfileDao { FirstName = "Test", LastName = "User2" },
                    History = new PracticeLogDao
                    {
                        KeyStatsJson = new Dictionary<char, KeyStatsDao>
                        {
                            { 'd', new KeyStatsDao { Key = "d", TypingCount = 45, CorrectCount = 40 } },
                            { 'f', new KeyStatsDao { Key = "f", TypingCount = 35, CorrectCount = 30 } }
                        }
                    },
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
            var account = await _context.Accounts.FindAsync(1);
            Assert.NotNull(account);
            account.AccountName = "Updated";

            // Act
            var result = await _repository.UpdateAccountAsync(account);

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
    }
}