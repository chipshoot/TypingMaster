using AutoMapper;
using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _mockRepository;
        private readonly Mock<IPracticeLogService> _mockPracticeLogService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _mockRepository = new Mock<IAccountRepository>();
            _mockPracticeLogService = new Mock<IPracticeLogService>();
            _mockCourseService = new Mock<ICourseService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _accountService = new AccountService(_mockRepository.Object, _mockPracticeLogService.Object, _mockCourseService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsAllMappedAccounts()
        {
            // Arrange
            var accountDaos = new List<AccountDao>
            {
                new AccountDao { Id = 1, AccountName = "Test1" },
                new AccountDao { Id = 2, AccountName = "Test2" }
            };

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Test1" },
                new Account { Id = 2, AccountName = "Test2" }
            };

            _mockRepository.Setup(repo => repo.GetAllAccountsAsync())
                .ReturnsAsync(accountDaos);

            _mockMapper.Setup(m => m.Map<Account>(It.IsAny<AccountDao>()))
                .Returns<AccountDao>(dao => accounts.First(a => a.Id == dao.Id));

            // Act
            var result = await _accountService.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            var enumerable = result as Account[] ?? result.ToArray();
            Assert.Equal(2, enumerable.Count());
            Assert.Equal("Test1", enumerable.First().AccountName);
            Assert.Equal("Test2", enumerable.Last().AccountName);
            _mockRepository.Verify(repo => repo.GetAllAccountsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAccounts_HandlesException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAccountsAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _accountService.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAccount_ReturnsCorrectAccount_WhenIdExists()
        {
            // Arrange
            var id = 1;
            var accountDao = new AccountDao { Id = id, AccountName = "Test1" };
            var account = new Account { Id = id, AccountName = "Test1" };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(id))
                .ReturnsAsync(accountDao);

            _mockMapper.Setup(m => m.Map<Account>(accountDao))
                .Returns(account);

            // Act
            var result = await _accountService.GetAccountById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Test1", result.AccountName);
            _mockRepository.Verify(repo => repo.GetAccountByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetAccount_ReturnsNull_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;
            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(id))
                .ReturnsAsync((AccountDao)null);

            // Act
            var result = await _accountService.GetAccountById(id);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetAccountByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetAccount_HandlesException()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(id))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _accountService.GetAccountById(id);

            // Assert
            Assert.Null(result);
            _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAccountByEmail_ReturnsCorrectAccount_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var accountDao = new AccountDao { Id = 1, AccountEmail = email };
            var account = new Account { Id = 1, AccountEmail = email };

            _mockRepository.Setup(repo => repo.GetAccountByEmailAsync(email))
                .ReturnsAsync(accountDao);

            _mockMapper.Setup(m => m.Map<Account>(accountDao))
                .Returns(account);

            // Act
            var result = await _accountService.GetAccountByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.AccountEmail);
            _mockRepository.Verify(repo => repo.GetAccountByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetAccountByEmail_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockRepository.Setup(repo => repo.GetAccountByEmailAsync(email))
                .ReturnsAsync((AccountDao)null);

            // Act
            var result = await _accountService.GetAccountByEmail(email);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetAccountByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_ReturnsCreatedAccount_WhenDataIsValid()
        {
            // Arrange
            var account = new Account
            {
                AccountName = "New Account",
                AccountEmail = "new@example.com",
                User = new UserProfile(),
                History = new PracticeLog()
            };

            var accountDao = new AccountDao
            {
                Id = 1,
                AccountName = "New Account",
                AccountEmail = "new@example.com"
            };

            var createdAccount = new Account
            {
                Id = 1,
                AccountName = "New Account",
                AccountEmail = "new@example.com"
            };

            _mockMapper.Setup(m => m.Map<AccountDao>(account))
                .Returns(accountDao);

            _mockRepository.Setup(repo => repo.CreateAccountAsync(accountDao))
                .ReturnsAsync(accountDao);

            _mockMapper.Setup(m => m.Map<Account>(accountDao))
                .Returns(createdAccount);

            // Act
            var result = await _accountService.CreateAccount(account);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Account", result.AccountName);
            Assert.Equal("new@example.com", result.AccountEmail);
            _mockRepository.Verify(repo => repo.CreateAccountAsync(accountDao), Times.Once);
        }

        [Theory]
        [InlineData(null, "new@example.com")]
        [InlineData("", "new@example.com")]
        [InlineData("New Account", null)]
        [InlineData("New Account", "")]
        public async Task CreateAccount_ReturnsNull_WhenDataIsInvalid(string name, string email)
        {
            // Arrange
            var account = new Account
            {
                AccountName = name,
                AccountEmail = email
            };

            // Act
            var result = await _accountService.CreateAccount(account);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.CreateAccountAsync(It.IsAny<AccountDao>()), Times.Never);
        }

        [Fact]
        public async Task CreateAccount_ReturnsNull_WhenAccountIsNull()
        {
            // Act
            var result = await _accountService.CreateAccount(null);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.CreateAccountAsync(It.IsAny<AccountDao>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsUpdatedAccount_WhenUpdateSucceeds()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                AccountName = "Updated Account",
                AccountEmail = "updated@example.com",
                Version = 1
            };

            var existingAccountDao = new AccountDao
            {
                Id = 1,
                AccountName = "Original Account",
                AccountEmail = "original@example.com",
                Version = 1
            };

            var updatedAccountDao = new AccountDao
            {
                Id = 1,
                AccountName = "Updated Account",
                AccountEmail = "updated@example.com",
                Version = 2
            };

            var updatedAccount = new Account
            {
                Id = 1,
                AccountName = "Updated Account",
                AccountEmail = "updated@example.com",
                Version = 2
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(account.Id))
                .ReturnsAsync(existingAccountDao);

            _mockMapper.Setup(m => m.Map<AccountDao>(account))
                .Returns(updatedAccountDao);

            _mockRepository.Setup(repo => repo.UpdateAccountAsync(updatedAccountDao))
                .ReturnsAsync(updatedAccountDao);

            _mockMapper.Setup(m => m.Map<Account>(updatedAccountDao))
                .Returns(updatedAccount);

            // Act
            var result = await _accountService.UpdateAccount(account);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Account", result.AccountName);
            Assert.Equal("updated@example.com", result.AccountEmail);
            Assert.Equal(2, result.Version);
            _mockRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsNull_WhenAccountNotFound()
        {
            // Arrange
            var account = new Account
            {
                Id = 999,
                AccountName = "Nonexistent Account",
                AccountEmail = "nonexistent@example.com"
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(account.Id))
                .ReturnsAsync((AccountDao)null);

            // Act
            var result = await _accountService.UpdateAccount(account);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsNull_WhenVersionConflict()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                AccountName = "Updated Account",
                AccountEmail = "updated@example.com",
                Version = 1
            };

            var existingAccountDao = new AccountDao
            {
                Id = 1,
                AccountName = "Original Account",
                AccountEmail = "original@example.com",
                Version = 2 // Higher version indicates someone else updated it
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(account.Id))
                .ReturnsAsync(existingAccountDao);

            // Act
            var result = await _accountService.UpdateAccount(account);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsTrue_WhenAccountExists()
        {
            // Arrange
            int id = 1;
            var accountDao = new AccountDao
            {
                Id = id,
                AccountName = "Account to Delete",
                AccountEmail = "delete@example.com"
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(id))
                .ReturnsAsync(accountDao);

            _mockRepository.Setup(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()))
                .ReturnsAsync(accountDao);

            // Act
            var result = await _accountService.DeleteAccount(id);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateAccountAsync(It.Is<AccountDao>(a =>
                a.IsDeleted == true &&
                a.DeletedAt != null &&
                a.AccountEmail.StartsWith("deleted-"))),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsFalse_WhenAccountDoesNotExist()
        {
            // Arrange
            int id = 999;
            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(id))
                .ReturnsAsync((AccountDao)null);

            // Act
            var result = await _accountService.DeleteAccount(id);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()), Times.Never);
        }

        [Fact]
        public async Task IsAccountUpdated_ReturnsTrue_WhenVersionIsOutdated()
        {
            // Arrange
            int accountId = 1;
            int version = 1;

            var accountDao = new AccountDao
            {
                Id = accountId,
                Version = 2 // Current version is newer
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(accountId))
                .ReturnsAsync(accountDao);

            // Act
            var result = await _accountService.IsAccountUpdated(accountId, version);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsAccountUpdated_ReturnsFalse_WhenVersionIsCurrent()
        {
            // Arrange
            int accountId = 1;
            int version = 2;

            var accountDao = new AccountDao
            {
                Id = accountId,
                Version = 2 // Same as provided version
            };

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(accountId))
                .ReturnsAsync(accountDao);

            // Act
            var result = await _accountService.IsAccountUpdated(accountId, version);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsAccountUpdated_ReturnsFalse_WhenAccountNotFound()
        {
            // Arrange
            int accountId = 999;
            int version = 1;

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(accountId))
                .ReturnsAsync((AccountDao)null);

            // Act
            var result = await _accountService.IsAccountUpdated(accountId, version);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetGuestAccount_ReturnsValidGuestAccount()
        {
            // Act
            var result = _accountService.GetGuestAccount();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("guest@test.com", result.AccountEmail);
            Assert.NotNull(result.AccountName);
            Assert.NotNull(result.History);
            Assert.NotNull(result.User);
        }

        [Fact]
        public async Task CreateAccount_ResetsInvalidCourseId_WhenHistoryCourseIdIsInvalid()
        {
            // Arrange
            var invalidCourseId = Guid.NewGuid();
            var account = new Account
            {
                AccountName = "Test Account",
                AccountEmail = "test@example.com",
                History = new PracticeLog { CurrentCourseId = invalidCourseId }
            };

            var accountDaoWithResetCourseId = new AccountDao
            {
                Id = 1,
                AccountName = "Test Account",
                AccountEmail = "test@example.com",
                History = new PracticeLogDao { CurrentCourseId = Guid.Empty }
            };

            var createdAccount = new Account
            {
                Id = 1,
                AccountName = "Test Account",
                AccountEmail = "test@example.com",
                History = new PracticeLog { CurrentCourseId = Guid.Empty }
            };

            // Setup course service to return null for the invalid course ID
            _mockCourseService.Setup(cs => cs.GetCourse(invalidCourseId))
                .ReturnsAsync((CourseDto)null);

            // Capture the mapped AccountDao for verification
            AccountDao capturedAccountDao = null;
            _mockMapper.Setup(m => m.Map<AccountDao>(account))
                .Returns<Account>(a =>
                {
                    capturedAccountDao = new AccountDao
                    {
                        AccountName = a.AccountName,
                        AccountEmail = a.AccountEmail,
                        History = new PracticeLogDao { CurrentCourseId = a.History.CurrentCourseId }
                    };
                    return capturedAccountDao;
                });

            _mockRepository.Setup(repo => repo.CreateAccountAsync(It.IsAny<AccountDao>()))
                .ReturnsAsync(accountDaoWithResetCourseId);

            _mockMapper.Setup(m => m.Map<Account>(accountDaoWithResetCourseId))
                .Returns(createdAccount);

            // Act
            var result = await _accountService.CreateAccount(account);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.History.CurrentCourseId);

            // Verify the course service was called to validate the course ID
            _mockCourseService.Verify(cs => cs.GetCourse(invalidCourseId), Times.Once);

            // Verify the course ID was reset before being sent to the repository
            Assert.NotNull(capturedAccountDao);
            Assert.Equal(Guid.Empty, capturedAccountDao.History.CurrentCourseId);
        }

        [Fact]
        public async Task UpdateAccount_ResetsInvalidCourseId_WhenHistoryCourseIdIsInvalid()
        {
            // Arrange
            var invalidCourseId = Guid.NewGuid();
            var accountId = 1;

            var existingAccountDao = new AccountDao
            {
                Id = accountId,
                AccountName = "Test Account",
                AccountEmail = "test@example.com",
                Version = 1,
                History = new PracticeLogDao { CurrentCourseId = Guid.Empty }
            };

            var accountToUpdate = new Account
            {
                Id = accountId,
                AccountName = "Updated Account",
                AccountEmail = "test@example.com",
                Version = 1,
                History = new PracticeLog { CurrentCourseId = invalidCourseId }
            };

            var updatedAccountDao = new AccountDao
            {
                Id = accountId,
                AccountName = "Updated Account",
                AccountEmail = "test@example.com",
                Version = 2,
                History = new PracticeLogDao { CurrentCourseId = Guid.Empty }
            };

            var updatedAccount = new Account
            {
                Id = accountId,
                AccountName = "Updated Account",
                AccountEmail = "test@example.com",
                Version = 2,
                History = new PracticeLog { CurrentCourseId = Guid.Empty }
            };

            // Setup course service to return null for the invalid course ID
            _mockCourseService.Setup(cs => cs.GetCourse(invalidCourseId))
                .ReturnsAsync((CourseDto)null);

            _mockRepository.Setup(repo => repo.GetAccountByIdAsync(accountId))
                .ReturnsAsync(existingAccountDao);

            // Capture the mapped AccountDao for verification
            AccountDao capturedAccountDao = null;
            _mockMapper.Setup(m => m.Map<AccountDao>(accountToUpdate))
                .Returns<Account>(a =>
                {
                    capturedAccountDao = new AccountDao
                    {
                        Id = a.Id,
                        AccountName = a.AccountName,
                        AccountEmail = a.AccountEmail,
                        Version = a.Version,
                        History = new PracticeLogDao { CurrentCourseId = a.History.CurrentCourseId }
                    };
                    return capturedAccountDao;
                });

            _mockRepository.Setup(repo => repo.UpdateAccountAsync(It.IsAny<AccountDao>()))
                .ReturnsAsync(updatedAccountDao);

            _mockMapper.Setup(m => m.Map<Account>(updatedAccountDao))
                .Returns(updatedAccount);

            // Act
            var result = await _accountService.UpdateAccount(accountToUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.History.CurrentCourseId);

            // Verify the course service was called to validate the course ID
            _mockCourseService.Verify(cs => cs.GetCourse(invalidCourseId), Times.Once);

            // Verify the course ID was reset before being sent to the repository
            Assert.NotNull(capturedAccountDao);
            Assert.Equal(Guid.Empty, capturedAccountDao.History.CurrentCourseId);
        }
    }
}