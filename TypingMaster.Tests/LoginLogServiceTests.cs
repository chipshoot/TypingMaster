using AutoMapper;
using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests
{
    public class LoginLogServiceTests
    {
        private readonly Mock<ILoginLogRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly LoginLogService _loginLogService;

        public LoginLogServiceTests()
        {
            _mockRepository = new Mock<ILoginLogRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _loginLogService = new LoginLogService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetLoginLogsByAccountIdAsync_ReturnsLoginLogs_WhenLogsExist()
        {
            // Arrange
            var accountId = 1;
            var now = DateTime.UtcNow;

            var loginLogDaos = new List<LoginLogDao>
            {
                new LoginLogDao
                {
                    Id = 1,
                    AccountId = accountId,
                    IpAddress = "192.168.1.1",
                    LoginTime = now.AddDays(-1),
                    IsSuccessful = true
                },
                new LoginLogDao
                {
                    Id = 2,
                    AccountId = accountId,
                    IpAddress = "192.168.1.2",
                    LoginTime = now.AddDays(-2),
                    IsSuccessful = true
                }
            };

            var loginLogs = new List<LoginLog>
            {
                new LoginLog
                {
                    Id = 1,
                    AccountId = accountId,
                    IpAddress = "192.168.1.1",
                    LoginTime = now.AddDays(-1),
                    IsSuccessful = true
                },
                new LoginLog
                {
                    Id = 2,
                    AccountId = accountId,
                    IpAddress = "192.168.1.2",
                    LoginTime = now.AddDays(-2),
                    IsSuccessful = true
                }
            };

            _mockRepository.Setup(repo => repo.GetLoginLogsByAccountIdAsync(accountId))
                .ReturnsAsync(loginLogDaos);

            _mockMapper.Setup(m => m.Map<LoginLog>(It.IsAny<LoginLogDao>()))
                .Returns<LoginLogDao>(dao => loginLogs.First(l => l.Id == dao.Id));

            // Act
            var result = await _loginLogService.GetLoginLogsByAccountIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("192.168.1.1", resultList[0].IpAddress);
            Assert.Equal("192.168.1.2", resultList[1].IpAddress);
            _mockRepository.Verify(repo => repo.GetLoginLogsByAccountIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetLoginLogsByAccountIdAsync_ReturnsEmptyList_WhenNoLogsExist()
        {
            // Arrange
            var accountId = 1;

            _mockRepository.Setup(repo => repo.GetLoginLogsByAccountIdAsync(accountId))
                .ReturnsAsync(new List<LoginLogDao>());

            // Act
            var result = await _loginLogService.GetLoginLogsByAccountIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetLoginLogsByAccountIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetLoginLogsByAccountIdAsync_HandlesException()
        {
            // Arrange
            var accountId = 1;

            _mockRepository.Setup(repo => repo.GetLoginLogsByAccountIdAsync(accountId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _loginLogService.GetLoginLogsByAccountIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateLoginLogAsync_ReturnsCreatedLog_WhenSuccessful()
        {
            // Arrange
            var accountId = 1;
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var isSuccessful = true;
            var loginTime = DateTime.UtcNow;

            var createdLoginLogDao = new LoginLogDao
            {
                Id = 1,
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = loginTime,
                IsSuccessful = isSuccessful,
                FailureReason = null
            };

            var mappedLoginLog = new LoginLog
            {
                Id = 1,
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = loginTime,
                IsSuccessful = isSuccessful,
                FailureReason = null
            };

            _mockRepository.Setup(repo => repo.CreateLoginLogAsync(It.IsAny<LoginLogDao>()))
                .ReturnsAsync(createdLoginLogDao);

            _mockMapper.Setup(m => m.Map<LoginLog>(createdLoginLogDao))
                .Returns(mappedLoginLog);

            // Act
            var result = await _loginLogService.CreateLoginLogAsync(accountId, ipAddress, userAgent, isSuccessful);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountId);
            Assert.Equal(ipAddress, result.IpAddress);
            Assert.Equal(userAgent, result.UserAgent);
            Assert.Equal(isSuccessful, result.IsSuccessful);
            Assert.Null(result.FailureReason);
            _mockRepository.Verify(repo => repo.CreateLoginLogAsync(It.IsAny<LoginLogDao>()), Times.Once);
        }

        [Fact]
        public async Task CreateLoginLogAsync_ReturnsLoginLog_WhenRepositoryThrowsException()
        {
            // Arrange
            var accountId = 1;
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var isSuccessful = true;

            _mockRepository.Setup(repo => repo.CreateLoginLogAsync(It.IsAny<LoginLogDao>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _loginLogService.CreateLoginLogAsync(accountId, ipAddress, userAgent, isSuccessful);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountId);
            Assert.Equal(ipAddress, result.IpAddress);
            Assert.Equal(userAgent, result.UserAgent);
            Assert.Equal(isSuccessful, result.IsSuccessful);
            Assert.Null(result.FailureReason);
            _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateLoginLogAsync_WithFailureReason_ReturnsCorrectLog()
        {
            // Arrange
            var accountId = 1;
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var isSuccessful = false;
            var failureReason = "Invalid credentials";
            var loginTime = DateTime.UtcNow;

            var createdLoginLogDao = new LoginLogDao
            {
                Id = 1,
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = loginTime,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason
            };

            var mappedLoginLog = new LoginLog
            {
                Id = 1,
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = loginTime,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason
            };

            _mockRepository.Setup(repo => repo.CreateLoginLogAsync(It.IsAny<LoginLogDao>()))
                .ReturnsAsync(createdLoginLogDao);

            _mockMapper.Setup(m => m.Map<LoginLog>(createdLoginLogDao))
                .Returns(mappedLoginLog);

            // Act
            var result = await _loginLogService.CreateLoginLogAsync(accountId, ipAddress, userAgent, isSuccessful, failureReason);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountId);
            Assert.Equal(ipAddress, result.IpAddress);
            Assert.Equal(userAgent, result.UserAgent);
            Assert.Equal(isSuccessful, result.IsSuccessful);
            Assert.Equal(failureReason, result.FailureReason);
            _mockRepository.Verify(repo => repo.CreateLoginLogAsync(It.IsAny<LoginLogDao>()), Times.Once);
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_ReturnsLogs_WhenLogsExist()
        {
            // Arrange
            var count = 2;
            var now = DateTime.UtcNow;

            var loginLogDaos = new List<LoginLogDao>
            {
                new LoginLogDao
                {
                    Id = 1,
                    AccountId = 1,
                    IpAddress = "192.168.1.1",
                    LoginTime = now,
                    IsSuccessful = true
                },
                new LoginLogDao
                {
                    Id = 2,
                    AccountId = 2,
                    IpAddress = "192.168.1.2",
                    LoginTime = now.AddMinutes(-30),
                    IsSuccessful = true
                }
            };

            var loginLogs = new List<LoginLog>
            {
                new LoginLog
                {
                    Id = 1,
                    AccountId = 1,
                    IpAddress = "192.168.1.1",
                    LoginTime = now,
                    IsSuccessful = true
                },
                new LoginLog
                {
                    Id = 2,
                    AccountId = 2,
                    IpAddress = "192.168.1.2",
                    LoginTime = now.AddMinutes(-30),
                    IsSuccessful = true
                }
            };

            _mockRepository.Setup(repo => repo.GetRecentLoginLogsAsync(count))
                .ReturnsAsync(loginLogDaos);

            _mockMapper.Setup(m => m.Map<LoginLog>(It.IsAny<LoginLogDao>()))
                .Returns<LoginLogDao>(dao => loginLogs.First(l => l.Id == dao.Id));

            // Act
            var result = await _loginLogService.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("192.168.1.1", resultList[0].IpAddress);
            Assert.Equal("192.168.1.2", resultList[1].IpAddress);
            _mockRepository.Verify(repo => repo.GetRecentLoginLogsAsync(count), Times.Once);
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_ReturnsEmptyList_WhenNoLogsExist()
        {
            // Arrange
            var count = 5;

            _mockRepository.Setup(repo => repo.GetRecentLoginLogsAsync(count))
                .ReturnsAsync(new List<LoginLogDao>());

            // Act
            var result = await _loginLogService.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetRecentLoginLogsAsync(count), Times.Once);
        }

        [Fact]
        public async Task GetRecentLoginLogsAsync_HandlesException()
        {
            // Arrange
            var count = 5;

            _mockRepository.Setup(repo => repo.GetRecentLoginLogsAsync(count))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _loginLogService.GetRecentLoginLogsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}