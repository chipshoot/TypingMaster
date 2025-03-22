using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests
{
    public class UserProfileRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserProfileRepository _repository;
        private readonly Mock<ILogger> _mockLogger;

        public UserProfileRepositoryTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"UserProfileTestDb_{Guid.NewGuid()}")
                .Options;

            // Create the in-memory database context
            _context = new ApplicationDbContext(options);

            // Setup test data
            SeedTestData();

            // Create logger mock
            _mockLogger = new Mock<ILogger>();

            // Create repository with real context and mock logger
            _repository = new UserProfileRepository(_context, _mockLogger.Object);
        }

        private void SeedTestData()
        {
            // Add test user profiles
            _context.UserProfiles.AddRange(
                new UserProfileDao
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Title = "Software Developer",
                    PhoneNumber = "123-456-7890",
                    AvatarUrl = "https://example.com/avatar1.jpg"
                },
                new UserProfileDao
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Title = "UX Designer",
                    PhoneNumber = "987-654-3210",
                    AvatarUrl = "https://example.com/avatar2.jpg"
                },
                new UserProfileDao
                {
                    Id = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Title = "Project Manager",
                    PhoneNumber = "555-123-4567",
                    AvatarUrl = "https://example.com/avatar3.jpg"
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
        public async Task GetUserProfileByIdAsync_ReturnsProfile_WhenIdExists()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _repository.GetUserProfileByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task GetUserProfileByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _repository.GetUserProfileByIdAsync(id);

            // Assert
            Assert.Null(result);
            Assert.True(!_repository.ProcessResult.HasErrors());
        }

        [Fact]
        public async Task CreateUserProfileAsync_AddsProfileToDatabase()
        {
            // Arrange
            var newProfile = new UserProfileDao
            {
                FirstName = "Alex",
                LastName = "Wilson",
                Title = "Data Scientist",
                PhoneNumber = "222-333-4444",
                AvatarUrl = "https://example.com/avatar4.jpg"
            };

            // Act
            var result = await _repository.CreateUserProfileAsync(newProfile);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newProfile.FirstName, result.FirstName);
            Assert.Equal(newProfile.LastName, result.LastName);
            Assert.Equal(newProfile.Title, result.Title);

            // Verify the profile was added to the database
            var savedProfile = await _context.UserProfiles.FindAsync(result.Id);
            Assert.NotNull(savedProfile);
            Assert.Equal("Alex", savedProfile.FirstName);
            Assert.Equal("Wilson", savedProfile.LastName);
        }

        [Fact]
        public async Task CreateUserProfileAsync_HandlesException()
        {
            // Arrange
            // Use a test DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ErrorTestDb_{Guid.NewGuid()}")
                .Options;

            // Create a wrapper to throw controlled exceptions
            var mockContext = new Mock<ApplicationDbContext>(options) { CallBase = true };
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var errorRepository = new UserProfileRepository(mockContext.Object, _mockLogger.Object);
            var newProfile = new UserProfileDao
            {
                FirstName = "Error",
                LastName = "Test"
            };

            // Act
            var result = await errorRepository.CreateUserProfileAsync(newProfile);

            // Assert
            Assert.Null(result);
            Assert.True(errorRepository.ProcessResult.HasErrors());
            Assert.Contains("Database error", errorRepository.ProcessResult.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Error(
                    It.Is<Exception>(e => e.Message.Contains("Database error")),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UpdatesExistingProfile()
        {
            // Arrange
            var existingProfile = await _context.UserProfiles.FindAsync(2);
            Assert.NotNull(existingProfile);

            var updatedProfile = new UserProfileDao
            {
                Id = existingProfile.Id,
                FirstName = "Jane",
                LastName = "Smith-Updated",
                Title = "Senior UX Designer",
                PhoneNumber = existingProfile.PhoneNumber,
                AvatarUrl = existingProfile.AvatarUrl
            };

            // Act
            var result = await _repository.UpdateUserProfileAsync(updatedProfile);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedProfile.LastName, result.LastName);
            Assert.Equal(updatedProfile.Title, result.Title);

            // Verify the change was saved to the database
            var updatedDbProfile = await _context.UserProfiles.FindAsync(2);
            Assert.NotNull(updatedDbProfile);
            Assert.Equal("Smith-Updated", updatedDbProfile.LastName);
            Assert.Equal("Senior UX Designer", updatedDbProfile.Title);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ReturnsNull_WhenProfileDoesNotExist()
        {
            // Arrange
            var nonExistentProfile = new UserProfileDao
            {
                Id = 999,
                FirstName = "NonExistent",
                LastName = "User"
            };

            // Act
            var result = await _repository.UpdateUserProfileAsync(nonExistentProfile);

            // Assert
            Assert.Null(result);
            Assert.True(_repository.ProcessResult.HasErrors());
            Assert.Contains("999", _repository.ProcessResult.ErrorMessage);
        }

        [Fact]
        public async Task DeleteUserProfileAsync_ReturnsFalse_WhenProfileDoesNotExist()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _repository.DeleteUserProfileAsync(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanDeleteUserProfileAsync_ReturnsFalse_WhenLinkedToActiveAccount()
        {
            // Arrange
            // Create a user profile
            var userProfile = new UserProfileDao
            {
                FirstName = "Active",
                LastName = "User",
                PhoneNumber = "111-222-3333"
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // Create an account linked to this user profile
            var account = new AccountDao
            {
                AccountName = "TestAccount",
                AccountEmail = "active@example.com",
                User = userProfile,
                IsDeleted = false, // Active account
                GoalStats = new StatsDao { Wpm = 50, Accuracy = 90 },
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var canDelete = await _repository.CanDeleteUserProfileAsync(userProfile.Id);

            // Assert
            Assert.False(canDelete);
        }

        [Fact]
        public async Task CanDeleteUserProfileAsync_ReturnsTrue_WhenLinkedToInactiveAccount()
        {
            // Arrange
            // Create a user profile
            var userProfile = new UserProfileDao
            {
                FirstName = "Inactive",
                LastName = "User",
                PhoneNumber = "444-555-6666"
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // Create an account linked to this user profile but marked as deleted
            var account = new AccountDao
            {
                AccountName = "DeletedAccount",
                AccountEmail = "deleted@example.com",
                User = userProfile,
                IsDeleted = true, // Inactive/deleted account
                DeletedAt = DateTime.UtcNow,
                GoalStats = new StatsDao { Wpm = 40, Accuracy = 85 },
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var canDelete = await _repository.CanDeleteUserProfileAsync(userProfile.Id);

            // Assert
            Assert.True(canDelete);
        }

        [Fact]
        public async Task DeleteUserProfileAsync_ReturnsFalse_WhenLinkedToActiveAccount()
        {
            // Arrange
            // Create a user profile
            var userProfile = new UserProfileDao
            {
                FirstName = "Cannot",
                LastName = "Delete",
                PhoneNumber = "777-888-9999"
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // Create an account linked to this user profile
            var account = new AccountDao
            {
                AccountName = "LinkedAccount",
                AccountEmail = "linked@example.com",
                User = userProfile,
                IsDeleted = false, // Active account
                GoalStats = new StatsDao { Wpm = 60, Accuracy = 95 },
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteUserProfileAsync(userProfile.Id);

            // Assert
            Assert.False(result);
            Assert.True(_repository.ProcessResult.HasErrors());
            Assert.Contains("linked to an active account", _repository.ProcessResult.ErrorMessage);

            // Verify that the profile wasn't deleted
            var profileStillExists = await _context.UserProfiles.FindAsync(userProfile.Id);
            Assert.NotNull(profileStillExists);
        }

        [Fact]
        public async Task DeleteUserProfileAsync_Succeeds_WhenLinkedToInactiveAccount()
        {
            // Arrange
            // Create a user profile
            var userProfile = new UserProfileDao
            {
                FirstName = "Can",
                LastName = "Delete",
                PhoneNumber = "000-111-2222"
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // Create an account linked to this user profile but marked as deleted
            var account = new AccountDao
            {
                AccountName = "InactiveAccount",
                AccountEmail = "inactive@example.com",
                User = userProfile,
                IsDeleted = true, // Inactive/deleted account
                DeletedAt = DateTime.UtcNow,
                GoalStats = new StatsDao { Wpm = 45, Accuracy = 88 },
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteUserProfileAsync(userProfile.Id);

            // Assert
            Assert.True(result);
            Assert.False(_repository.ProcessResult.HasErrors());

            // Verify the profile was actually deleted
            var deletedProfile = await _context.UserProfiles.FindAsync(userProfile.Id);
            Assert.Null(deletedProfile);
        }
    }
}