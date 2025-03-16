using Xunit;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using TypingMaster.Business.Models;
using TypingMaster.Server.Dao;
using TypingMaster.Server.Mapping;

namespace TypingMaster.Tests
{
    public class AccountMapProfileTests
    {
        private readonly IMapper _mapper;

        public AccountMapProfileTests()
        {
            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AccountMapProfile>();
            });
            
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void AccountDao_To_Account_MapsCorrectly()
        {
            // Arrange
            var accountDao = new AccountDao
            {
                Id = 1,
                AccountName = "TestAccount",
                AccountEmail = "test@example.com",
                GoalStats = new StatsDao { Wpm = 80, Accuracy = 95 },
                User = new UserProfileDao
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Title = "Developer",
                    PhoneNumber = "123-456-7890",
                    AvatarUrl = "https://example.com/avatar.jpg"
                },
                History = new PracticeLogDao
                {
                    Id = 1,
                    CurrentCourseId = Guid.NewGuid(),
                    CurrentLessonId = 1,
                    PracticeStats = new List<DrillStatsDao>(),
                    KeyStatsJson = new Dictionary<char, KeyStatsDao>(),
                    PracticeDuration = 60
                },
                Courses = new List<CourseDao>
                {
                    new CourseDao
                    {
                        Id = Guid.NewGuid(),
                        AccountId = 1,
                        Name = "Test Course",
                        LessonDataUrl = "https://example.com/lesson-data",
                        Description = "A test course",
                        Type = "Standard",
                        Settings = "{}"
                    }
                }
            };

            // Act
            var account = _mapper.Map<Account>(accountDao);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(accountDao.Id, account.Id);
            Assert.Equal(accountDao.AccountName, account.AccountName);
            Assert.Equal(accountDao.AccountEmail, account.AccountEmail);
            
            // Test GoalStats
            Assert.NotNull(account.GoalStats);
            Assert.Equal(accountDao.GoalStats.Wpm, account.GoalStats.Wpm);
            Assert.Equal(accountDao.GoalStats.Accuracy, account.GoalStats.Accuracy);
            
            // Test User
            Assert.NotNull(account.User);
            Assert.Equal(accountDao.User.Id, account.User.Id);
            Assert.Equal(accountDao.User.FirstName, account.User.FirstName);
            Assert.Equal(accountDao.User.LastName, account.User.LastName);
            Assert.Equal(accountDao.User.Title, account.User.Title);
            Assert.Equal(accountDao.User.PhoneNumber, account.User.PhoneNumber);
            Assert.Equal(accountDao.User.AvatarUrl, account.User.AvatarUrl);
            
            // Test History
            Assert.NotNull(account.History);
            Assert.Equal(accountDao.History.Id, account.History.Id);
            Assert.Equal(accountDao.History.CurrentCourseId, account.History.CurrentCourseId);
            Assert.Equal(accountDao.History.CurrentLessonId, account.History.CurrentLessonId);
            Assert.Equal(accountDao.History.PracticeDuration, account.History.PracticeDuration);
            
            // CourseId, TestCourseId, and GameCourseId should be default values as they're ignored in the mapping
            Assert.Equal(default, account.CourseId);
            Assert.Equal(default, account.TestCourseId);
            Assert.Equal(default, account.GameCourseId);
        }

        [Fact]
        public void Account_To_AccountDao_MapsCorrectly()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                AccountName = "TestAccount",
                AccountEmail = "test@example.com",
                GoalStats = new StatsBase { Wpm = 80, Accuracy = 95 },
                User = new UserProfile
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Title = "Developer",
                    PhoneNumber = "123-456-7890",
                    AvatarUrl = "https://example.com/avatar.jpg"
                },
                History = new PracticeLog
                {
                    Id = 1,
                    CurrentCourseId = Guid.NewGuid(),
                    CurrentLessonId = 1,
                    PracticeStats = new List<DrillStats>(),
                    KeyStats = new Dictionary<char, KeyStats>(),
                    PracticeDuration = 60
                },
                CourseId = Guid.NewGuid(),
                TestCourseId = Guid.NewGuid(),
                GameCourseId = Guid.NewGuid()
            };

            // Act
            var accountDao = _mapper.Map<AccountDao>(account);

            // Assert
            Assert.NotNull(accountDao);
            Assert.Equal(account.Id, accountDao.Id);
            Assert.Equal(account.AccountName, accountDao.AccountName);
            Assert.Equal(account.AccountEmail, accountDao.AccountEmail);
            
            // Test GoalStats
            Assert.NotNull(accountDao.GoalStats);
            Assert.Equal(account.GoalStats.Wpm, accountDao.GoalStats.Wpm);
            Assert.Equal(account.GoalStats.Accuracy, accountDao.GoalStats.Accuracy);
            
            // Test User
            Assert.NotNull(accountDao.User);
            Assert.Equal(account.User.Id, accountDao.User.Id);
            Assert.Equal(account.User.FirstName, accountDao.User.FirstName);
            Assert.Equal(account.User.LastName, accountDao.User.LastName);
            Assert.Equal(account.User.Title, accountDao.User.Title);
            Assert.Equal(account.User.PhoneNumber, accountDao.User.PhoneNumber);
            Assert.Equal(account.User.AvatarUrl, accountDao.User.AvatarUrl);
            
            // Test History
            Assert.NotNull(accountDao.History);
            Assert.Equal(account.History.Id, accountDao.History.Id);
            Assert.Equal(account.History.CurrentCourseId, accountDao.History.CurrentCourseId);
            Assert.Equal(account.History.CurrentLessonId, accountDao.History.CurrentLessonId);
            Assert.Equal(account.History.PracticeDuration, accountDao.History.PracticeDuration);
            
            // Courses should be initialized but empty as they're ignored in the mapping
            Assert.NotNull(accountDao.Courses);
            Assert.Empty(accountDao.Courses);
        }

        [Fact]
        public void DrillStatsDao_To_DrillStats_MapsTrainingTypeCorrectly()
        {
            // Arrange
            var drillStatsDao = new DrillStatsDao
            {
                Id = 1,
                PracticeLogId = 1,
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                PracticeText = "Test text",
                TypedText = "Test typed text",
                KeyEventsJson = new Queue<KeyEventDao>(),
                Wpm = 80,
                Accuracy = 95.5,
                TrainingType = 2, // Should map to TrainingType.SpeedTest
                StartTime = DateTime.UtcNow,
                FinishTime = DateTime.UtcNow.AddMinutes(5)
            };

            // Act
            var drillStats = _mapper.Map<DrillStats>(drillStatsDao);

            // Assert
            Assert.NotNull(drillStats);
            Assert.Equal(drillStatsDao.Id, drillStats.Id);
            Assert.Equal(drillStatsDao.PracticeLogId, drillStats.PracticeLogId);
            Assert.Equal(drillStatsDao.CourseId, drillStats.CourseId);
            Assert.Equal(drillStatsDao.LessonId, drillStats.LessonId);
            Assert.Equal(drillStatsDao.PracticeText, drillStats.PracticeText);
            Assert.Equal(drillStatsDao.TypedText, drillStats.TypedText);
            Assert.Equal(drillStatsDao.Wpm, drillStats.Wpm);
            Assert.Equal(drillStatsDao.Accuracy, drillStats.Accuracy);
            
            // Test that TrainingType enum maps correctly
            Assert.Equal(TrainingType.SpeedTest, drillStats.Type);
            
            Assert.Equal(drillStatsDao.StartTime, drillStats.StartTime);
            Assert.Equal(drillStatsDao.FinishTime, drillStats.FinishTime);
        }

        [Fact]
        public void DrillStats_To_DrillStatsDao_MapsTrainingTypeCorrectly()
        {
            // Arrange
            var drillStats = new DrillStats
            {
                Id = 1,
                PracticeLogId = 1,
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                PracticeText = "Test text",
                TypedText = "Test typed text",
                KeyEvents = new Queue<KeyEvent>(),
                Wpm = 80,
                Accuracy = 95.5,
                Type = TrainingType.Game, // Should map to 3
                StartTime = DateTime.UtcNow,
                FinishTime = DateTime.UtcNow.AddMinutes(5)
            };

            // Act
            var drillStatsDao = _mapper.Map<DrillStatsDao>(drillStats);

            // Assert
            Assert.NotNull(drillStatsDao);
            Assert.Equal(drillStats.Id, drillStatsDao.Id);
            Assert.Equal(drillStats.PracticeLogId, drillStatsDao.PracticeLogId);
            Assert.Equal(drillStats.CourseId, drillStatsDao.CourseId);
            Assert.Equal(drillStats.LessonId, drillStatsDao.LessonId);
            Assert.Equal(drillStats.PracticeText, drillStatsDao.PracticeText);
            Assert.Equal(drillStats.TypedText, drillStatsDao.TypedText);
            Assert.Equal(drillStats.Wpm, drillStatsDao.Wpm);
            Assert.Equal(drillStats.Accuracy, drillStatsDao.Accuracy);
            
            // Test that TrainingType int maps correctly
            Assert.Equal(3, drillStatsDao.TrainingType); // TrainingType.Game is 3
            
            Assert.Equal(drillStats.StartTime, drillStatsDao.StartTime);
            Assert.Equal(drillStats.FinishTime, drillStatsDao.FinishTime);
        }

        [Fact]
        public void CourseDao_Maps_Correctly()
        {
            // Arrange
            var courseDao = new CourseDao
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                Name = "Test Course",
                LessonDataUrl = "https://example.com/lesson-data",
                Description = "A test course",
                Type = "Standard",
                Settings = "{}"
            };

            // TODO: Add Course mapping tests when Course model is available
            // Currently, there's no direct mapping between CourseDao and Course in the AccountMapProfile
        }
    }
}
