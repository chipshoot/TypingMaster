using AutoMapper;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;
using TypingMaster.Business.Mapping;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.Tests
{
    public class DomainMapProfileTests
    {
        private readonly IMapper _mapper;

        public DomainMapProfileTests()
        {
            // Configure AutoMapper
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DomainMapProfile>(); });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void AccountDao_To_Account_MapsCorrectly()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var testCourseId = Guid.NewGuid();
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
                        Id = courseId,
                        AccountId = 1,
                        Name = "Beginner Course",
                        LessonDataUrl = "https://example.com/lesson-data",
                        Description = "A test course",
                        Type = "0",
                        SettingsJson = new CourseSettingDao
                        {
                            Minutes = 30,
                            NewKeysPerStep = 1,
                            PracticeTextLength = 50,
                            TargetStats = new StatsDao
                            {
                                Wpm = 50,
                                Accuracy = 90
                            }
                        }
                    },
                    new CourseDao
                    {
                        Id = testCourseId,
                        AccountId = 1,
                        Name = "Test Course",
                        LessonDataUrl = "https://example.com/lesson-data",
                        Description = "A test course",
                        Type = "1",
                        SettingsJson = new CourseSettingDao
                        {
                            Minutes = 30,
                            NewKeysPerStep = 1,
                            PracticeTextLength = 50,
                            TargetStats = new StatsDao
                            {
                                Wpm = 50,
                                Accuracy = 90
                            }
                        }
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

            // Test Course ID mappings
            // Type "0" corresponds to TrainingType.Course
            var regularCourse = accountDao.Courses.First(c => c.Type == "0");
            Assert.Equal(regularCourse.Id, account.CourseId);

            // Type "1" corresponds to TrainingType.AllKeysTest
            var testCourse = accountDao.Courses.First(c => c.Type == "1");
            Assert.Equal(testCourse.Id, account.TestCourseId);

            // Type "3" corresponds to TrainingType.Game (if present)
            var gameCourse = accountDao.Courses.FirstOrDefault(c => c.Type == "3");
            Assert.Equal(gameCourse?.Id ?? default, account.GameCourseId);
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

            // Test Courses mapping
            Assert.NotNull(accountDao.Courses);
            Assert.Equal(3, accountDao.Courses.Count); // Should map all three course types

            // Verify regular course mapping
            var regularCourse = accountDao.Courses.Single(c => c.Type == "0"); // TrainingType.Course
            Assert.Equal(account.CourseId, regularCourse.Id);
            Assert.Equal(account.Id, regularCourse.AccountId);

            // Verify test course mapping
            var testCourse = accountDao.Courses.Single(c => c.Type == "1"); // TrainingType.AllKeysTest
            Assert.Equal(account.TestCourseId, testCourse.Id);
            Assert.Equal(account.Id, testCourse.AccountId);

            // Verify game course mapping
            var gameCourse = accountDao.Courses.Single(c => c.Type == "3"); // TrainingType.Game
            Assert.Equal(account.GameCourseId, gameCourse.Id);
            Assert.Equal(account.Id, gameCourse.AccountId);

            // Courses should be initialized but empty as they're ignored in the mapping
            Assert.NotNull(accountDao.Courses);
            Assert.NotEmpty(accountDao.Courses);
        }

        [Fact]
        public void Account_To_AccountDao_EmptyGuids_NotAddedToCourses()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                AccountName = "TestAccount",
                AccountEmail = "test@example.com",
                User = new UserProfile
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User"
                },
                History = new PracticeLog
                {
                    Id = 1,
                    PracticeDuration = 60
                },
                // Set some valid GUIDs and some empty ones
                CourseId = Guid.NewGuid(),        // Valid GUID (should be added)
                TestCourseId = Guid.Empty,        // Empty GUID (should NOT be added)
                GameCourseId = Guid.NewGuid()     // Valid GUID (should be added)
            };

            // Act
            var accountDao = _mapper.Map<AccountDao>(account);

            // Assert
            Assert.NotNull(accountDao);
            Assert.NotNull(accountDao.Courses);

            // Should only have 2 courses (CourseId and GameCourseId), TestCourseId should be excluded
            Assert.Equal(2, accountDao.Courses.Count);

            // Verify only the expected courses exist
            Assert.Single(accountDao.Courses.Where(c => c.Type == "0")); // Regular course
            Assert.Empty(accountDao.Courses.Where(c => c.Type == "1"));  // Test course should not be present
            Assert.Single(accountDao.Courses.Where(c => c.Type == "3")); // Game course

            // Verify the correct IDs were mapped
            var regularCourse = accountDao.Courses.Single(c => c.Type == "0");
            Assert.Equal(account.CourseId, regularCourse.Id);

            var gameCourse = accountDao.Courses.Single(c => c.Type == "3");
            Assert.Equal(account.GameCourseId, gameCourse.Id);
        }

        [Fact]
        public void Account_To_AccountDao_AllEmptyGuids_ResultsInEmptyCourses()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                AccountName = "TestAccount",
                AccountEmail = "test@example.com",
                User = new UserProfile
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User"
                },
                History = new PracticeLog
                {
                    Id = 1,
                    PracticeDuration = 60
                },
                // Set all GUIDs to empty
                CourseId = Guid.Empty,
                TestCourseId = Guid.Empty,
                GameCourseId = Guid.Empty
            };

            // Act
            var accountDao = _mapper.Map<AccountDao>(account);

            // Assert
            Assert.NotNull(accountDao);
            Assert.NotNull(accountDao.Courses);

            // Courses collection should be empty when all GUIDs are empty
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
        public void CourseDao_To_CourseBase_MapsCorrectly()
        {
            // Arrange
            var courseDao = new CourseDao
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                Name = "BeginnerCourse",
                Type = "0", // TrainingType.Course
                SettingsJson = new CourseSettingDao
                {
                    Minutes = 30,
                    NewKeysPerStep = 1,
                    PracticeTextLength = 50,
                    TargetStats = new StatsDao
                    {
                        Wpm = 50,
                        Accuracy = 90
                    }
                }
            };

            // Act
            var courseBase = _mapper.Map(courseDao, typeof(CourseDao), GetCourseTypeFromName(courseDao.Name));

            // Assert
            Assert.NotNull(courseBase);
            Assert.IsType<BeginnerCourse>(courseBase);
            var course = (BeginnerCourse)courseBase;
            Assert.Equal(courseDao.Id, course.Id);
            Assert.Equal(courseDao.Name, course.Name);
            Assert.Equal(TrainingType.Course, course.Type);

            // Test Settings mapping
            Assert.NotNull(course.Settings);
            Assert.Equal(courseDao.SettingsJson.Minutes, course.Settings.Minutes);
            Assert.Equal(courseDao.SettingsJson.NewKeysPerStep, course.Settings.NewKeysPerStep);
            Assert.Equal(courseDao.SettingsJson.PracticeTextLength, course.Settings.PracticeTextLength);
            Assert.NotNull(course.Settings.TargetStats);
            Assert.Equal(courseDao.SettingsJson.TargetStats.Wpm, course.Settings.TargetStats.Wpm);
            Assert.Equal(courseDao.SettingsJson.TargetStats.Accuracy, course.Settings.TargetStats.Accuracy);
        }

        private Type GetCourseTypeFromName(string name)
        {
            return name switch
            {
                "BeginnerCourse" => typeof(BeginnerCourse),
                "AdvancedLevelCourse" => typeof(AdvancedLevelCourse),
                _ => typeof(AdvancedLevelCourse) // Default to AdvancedLevelCourse if name doesn't match
            };
        }

        [Fact]
        public void CourseBase_To_CourseDao_MapsCorrectly()
        {
            // Arrange
            var course = new AdvancedLevelCourse
            {
                Id = Guid.NewGuid(),
                Name = "AdvancedLevelCourse",
                Settings = new CourseSetting
                {
                    Minutes = 30,
                    NewKeysPerStep = 1,
                    PracticeTextLength = 50,
                    TargetStats = new StatsBase
                    {
                        Wpm = 50,
                        Accuracy = 90
                    }
                }
            };

            // Act
            var courseDao = _mapper.Map<CourseDao>(course);

            // Assert
            Assert.NotNull(courseDao);
            Assert.Equal(course.Id, courseDao.Id);
            Assert.Equal(course.Name, courseDao.Name);
            Assert.Equal(((int)course.Type).ToString(), courseDao.Type);

            // Test Settings mapping
            Assert.NotNull(courseDao.SettingsJson);
            Assert.Equal(course.Settings.Minutes, courseDao.SettingsJson.Minutes);
            Assert.Equal(course.Settings.NewKeysPerStep, courseDao.SettingsJson.NewKeysPerStep);
            Assert.Equal(course.Settings.PracticeTextLength, courseDao.SettingsJson.PracticeTextLength);
            Assert.NotNull(courseDao.SettingsJson.TargetStats);
            Assert.Equal(course.Settings.TargetStats.Wpm, courseDao.SettingsJson.TargetStats.Wpm);
            Assert.Equal(course.Settings.TargetStats.Accuracy, courseDao.SettingsJson.TargetStats.Accuracy);
        }

        [Fact]
        public void CourseBase_To_CourseDao_HandlesNullSettings()
        {
            // Arrange
            var course = new AdvancedLevelCourse
            {
                Id = Guid.NewGuid(),
                Name = "AdvancedLevelCourse",
                Settings = null
            };

            // Act
            var courseDao = _mapper.Map<CourseDao>(course);

            // Assert
            Assert.NotNull(courseDao);
            Assert.Equal(course.Id, courseDao.Id);
            Assert.Equal(course.Name, courseDao.Name);
            Assert.Equal(((int)course.Type).ToString(), courseDao.Type);
            Assert.Null(courseDao.SettingsJson);
        }

        [Fact]
        public void KeyEventDao_To_KeyEvent_MapsCorrectly()
        {
            // Arrange
            var keyEventDao = new KeyEventDao
            {
                Key = 'a',
                TypedKey = 'b',
                IsCorrect = false,
                KeyDownTime = DateTime.UtcNow,
                KeyUpTime = DateTime.UtcNow.AddMilliseconds(100),
                Latency = 50.5f
            };

            // Act
            var keyEvent = _mapper.Map<KeyEvent>(keyEventDao);

            // Assert
            Assert.NotNull(keyEvent);
            Assert.Equal(keyEventDao.Key, keyEvent.Key);
            Assert.Equal(keyEventDao.TypedKey, keyEvent.TypedKey);
            Assert.Equal(keyEventDao.IsCorrect, keyEvent.IsCorrect);
            Assert.Equal(keyEventDao.KeyDownTime, keyEvent.KeyDownTime);
            Assert.Equal(keyEventDao.KeyUpTime, keyEvent.KeyUpTime);
            Assert.Equal(keyEventDao.Latency, keyEvent.Latency);
        }

        [Fact]
        public void KeyEvent_To_KeyEventDao_MapsCorrectly()
        {
            // Arrange
            var keyEvent = new KeyEvent
            {
                Key = 'x',
                TypedKey = 'y',
                IsCorrect = true,
                KeyDownTime = DateTime.UtcNow,
                KeyUpTime = DateTime.UtcNow.AddMilliseconds(150),
                Latency = 75.3f
            };

            // Act
            var keyEventDao = _mapper.Map<KeyEventDao>(keyEvent);

            // Assert
            Assert.NotNull(keyEventDao);
            Assert.Equal(keyEvent.Key, keyEventDao.Key);
            Assert.Equal(keyEvent.TypedKey, keyEventDao.TypedKey);
            Assert.Equal(keyEvent.IsCorrect, keyEventDao.IsCorrect);
            Assert.Equal(keyEvent.KeyDownTime, keyEventDao.KeyDownTime);
            Assert.Equal(keyEvent.KeyUpTime, keyEventDao.KeyUpTime);
            Assert.Equal(keyEvent.Latency, keyEventDao.Latency);
        }

        [Fact]
        public void KeyStatsDao_To_KeyStats_MapsCorrectly()
        {
            // Arrange
            var keyStatsDao = new KeyStatsDao
            {
                Key = "a",
                TypingCount = 100,
                CorrectCount = 95,
                PressDuration = 150.5,
                Latency = 50.2,
                Wpm = 60,
                Accuracy = 95.0
            };

            // Act
            var keyStats = _mapper.Map<KeyStats>(keyStatsDao);

            // Assert
            Assert.NotNull(keyStats);
            Assert.Equal(keyStatsDao.Key, keyStats.Key);
            Assert.Equal(keyStatsDao.TypingCount, keyStats.TypingCount);
            Assert.Equal(keyStatsDao.CorrectCount, keyStats.CorrectCount);
            Assert.Equal(keyStatsDao.PressDuration, keyStats.PressDuration);
            Assert.Equal(keyStatsDao.Latency, keyStats.Latency);
            Assert.Equal(keyStatsDao.Wpm, keyStats.Wpm);
            Assert.Equal(keyStatsDao.Accuracy, keyStats.Accuracy);
        }

        [Fact]
        public void KeyStats_To_KeyStatsDao_MapsCorrectly()
        {
            // Arrange
            var keyStats = new KeyStats
            {
                Key = "b",
                TypingCount = 200,
                CorrectCount = 180,
                PressDuration = 120.3,
                Latency = 45.7,
                Wpm = 75,
                Accuracy = 90.0
            };

            // Act
            var keyStatsDao = _mapper.Map<KeyStatsDao>(keyStats);

            // Assert
            Assert.NotNull(keyStatsDao);
            Assert.Equal(keyStats.Key, keyStatsDao.Key);
            Assert.Equal(keyStats.TypingCount, keyStatsDao.TypingCount);
            Assert.Equal(keyStats.CorrectCount, keyStatsDao.CorrectCount);
            Assert.Equal(keyStats.PressDuration, keyStatsDao.PressDuration);
            Assert.Equal(keyStats.Latency, keyStatsDao.Latency);
            Assert.Equal(keyStats.Wpm, keyStatsDao.Wpm);
            Assert.Equal(keyStats.Accuracy, keyStatsDao.Accuracy);
        }

        [Fact]
        public void StatsDao_To_StatsBase_MapsCorrectly()
        {
            // Arrange
            var statsDao = new StatsDao
            {
                Wpm = 65,
                Accuracy = 92.5
            };

            // Act
            var statsBase = _mapper.Map<StatsBase>(statsDao);

            // Assert
            Assert.NotNull(statsBase);
            Assert.Equal(statsDao.Wpm, statsBase.Wpm);
            Assert.Equal(statsDao.Accuracy, statsBase.Accuracy);
        }

        [Fact]
        public void StatsBase_To_StatsDao_MapsCorrectly()
        {
            // Arrange
            var statsBase = new StatsBase
            {
                Wpm = 80,
                Accuracy = 88.5
            };

            // Act
            var statsDao = _mapper.Map<StatsDao>(statsBase);

            // Assert
            Assert.NotNull(statsDao);
            Assert.Equal(statsBase.Wpm, statsDao.Wpm);
            Assert.Equal(statsBase.Accuracy, statsDao.Accuracy);
        }

        [Fact]
        public void PracticeLogDao_To_PracticeLog_MapsCorrectly()
        {
            // Arrange
            var practiceLogDao = new PracticeLogDao
            {
                Id = 1,
                CurrentCourseId = Guid.NewGuid(),
                CurrentLessonId = 5,
                PracticeStats = new List<DrillStatsDao>
                {
                    new DrillStatsDao
                    {
                        Id = 1,
                        PracticeLogId = 1,
                        Wpm = 70,
                        Accuracy = 93.5,
                        TrainingType = 1,
                        PracticeText = "actual text",
                        TypedText = "typed text",
                        StartTime = DateTime.Now,
                        KeyEventsJson = new Queue<KeyEventDao>()
                    }
                },
                KeyStatsJson = new Dictionary<char, KeyStatsDao>
                {
                    {'a', new KeyStatsDao { Key = "a", TypingCount = 50, CorrectCount = 45 }}
                },
                PracticeDuration = 3600
            };

            // Act
            var practiceLog = _mapper.Map<PracticeLog>(practiceLogDao);

            // Assert
            Assert.NotNull(practiceLog);
            Assert.Equal(practiceLogDao.Id, practiceLog.Id);
            Assert.Equal(practiceLogDao.CurrentCourseId, practiceLog.CurrentCourseId);
            Assert.Equal(practiceLogDao.CurrentLessonId, practiceLog.CurrentLessonId);
            Assert.Equal(practiceLogDao.PracticeDuration, practiceLog.PracticeDuration);

            // Check PracticeStats mapping
            Assert.NotNull(practiceLog.PracticeStats);
            Assert.Single(practiceLog.PracticeStats);
            var firstStat = practiceLog.PracticeStats.First();
            var firstStatDao = practiceLogDao.PracticeStats.First();
            Assert.Equal(firstStatDao.Wpm, firstStat.Wpm);
            Assert.Equal(firstStatDao.Accuracy, firstStat.Accuracy);

            // Check KeyStats mapping
            Assert.NotNull(practiceLog.KeyStats);
            Assert.Single(practiceLog.KeyStats);
            Assert.True(practiceLog.KeyStats.ContainsKey('a'));
            var keyStats = practiceLog.KeyStats['a'];
            var keyStatsDao = practiceLogDao.KeyStatsJson['a'];
            Assert.Equal(keyStatsDao.TypingCount, keyStats.TypingCount);
            Assert.Equal(keyStatsDao.CorrectCount, keyStats.CorrectCount);
        }

        [Fact]
        public void PracticeLog_To_PracticeLogDao_MapsCorrectly()
        {
            // Arrange
            var practiceLog = new PracticeLog
            {
                Id = 1,
                CurrentCourseId = Guid.NewGuid(),
                CurrentLessonId = 5,
                PracticeStats = new List<DrillStats>
                {
                    new DrillStats
                    {
                        Id = 1,
                        PracticeLogId = 1,
                        CourseId = Guid.NewGuid(), // Ensure we have a non-empty CourseId
                        Wpm = 70,
                        Accuracy = 93.5,
                        Type = TrainingType.Course,
                        FinishTime = DateTime.Now,
                        PracticeText = "actual text",
                        TypedText = "typed text",
                        KeyEvents = new Queue<KeyEvent>()
                    }
                },
                KeyStats = new Dictionary<char, KeyStats>
                {
                    {'a', new KeyStats { Key = "a", TypingCount = 50, CorrectCount = 45 }}
                },
                PracticeDuration = 3600
            };

            // Act
            var practiceLogDao = _mapper.Map<PracticeLogDao>(practiceLog);

            // Assert
            Assert.NotNull(practiceLogDao);
            Assert.Equal(practiceLog.Id, practiceLogDao.Id);
            Assert.Equal(practiceLog.CurrentCourseId, practiceLogDao.CurrentCourseId);
            Assert.Equal(practiceLog.CurrentLessonId, practiceLogDao.CurrentLessonId);
            Assert.Equal(practiceLog.PracticeDuration, practiceLogDao.PracticeDuration);

            // Check PracticeStats mapping
            Assert.NotNull(practiceLogDao.PracticeStats);
            Assert.Single(practiceLogDao.PracticeStats);
            var firstStatDao = practiceLogDao.PracticeStats.First();
            var firstStat = practiceLog.PracticeStats.First();
            Assert.Equal(firstStat.Wpm, firstStatDao.Wpm);
            Assert.Equal(firstStat.Accuracy, firstStatDao.Accuracy);

            // Check KeyStats mapping
            Assert.NotNull(practiceLogDao.KeyStatsJson);
            Assert.Single(practiceLogDao.KeyStatsJson);
            Assert.True(practiceLogDao.KeyStatsJson.ContainsKey('a'));
            var keyStatsDao = practiceLogDao.KeyStatsJson['a'];
            var keyStats = practiceLog.KeyStats['a'];
            Assert.Equal(keyStats.TypingCount, keyStatsDao.TypingCount);
            Assert.Equal(keyStats.CorrectCount, keyStatsDao.CorrectCount);
        }

        [Theory]
        [InlineData(TrainingType.Course, 0)]
        [InlineData(TrainingType.AllKeysTest, 1)]
        [InlineData(TrainingType.SpeedTest, 2)]
        [InlineData(TrainingType.Game, 3)]
        public void TrainingType_Maps_Correctly_In_Both_Directions(TrainingType enumValue, int daoValue)
        {
            // Arrange
            var drillStats = GetTestDrillStatsWithType(enumValue);
            var drillStatsDao = GetTestDrillStatsDaoWithType(daoValue);

            // Act & Assert
            // Test enum to int mapping
            var mappedDao = _mapper.Map<DrillStatsDao>(drillStats);
            Assert.Equal(daoValue, mappedDao.TrainingType);

            // Test int to enum mapping
            var mappedModel = _mapper.Map<DrillStats>(drillStatsDao);
            Assert.Equal(enumValue, mappedModel.Type);
        }

        private DrillStats GetTestDrillStatsWithType(TrainingType type)
        {
            return new DrillStats
            {
                Id = 1,
                PracticeLogId = 1,
                Wpm = 70,
                Accuracy = 93.5,
                Type = type,
                FinishTime = DateTime.Now,
                PracticeText = "actual text",
                TypedText = "typed text",
                KeyEvents = new Queue<KeyEvent>()
            };
        }

        private DrillStatsDao GetTestDrillStatsDaoWithType(int typeNum)
        {
            return new DrillStatsDao
            {
                Id = 1,
                PracticeLogId = 1,
                Wpm = 70,
                Accuracy = 93.5,
                TrainingType = typeNum,
                PracticeText = "actual text",
                TypedText = "typed text",
                StartTime = DateTime.Now,
                KeyEventsJson = new Queue<KeyEventDao>()
            };
        }

        [Fact]
        public void LoginLogDao_To_LoginLog_MapsCorrectly()
        {
            // Arrange
            var loginLogDao = new LoginLogDao
            {
                Id = 100,
                AccountId = 1,
                IpAddress = "192.168.1.1",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                LoginTime = DateTime.UtcNow,
                IsSuccessful = true,
                FailureReason = null
            };

            // Act
            var loginLog = _mapper.Map<LoginLog>(loginLogDao);

            // Assert
            Assert.NotNull(loginLog);
            Assert.Equal(loginLogDao.Id, loginLog.Id);
            Assert.Equal(loginLogDao.AccountId, loginLog.AccountId);
            Assert.Equal(loginLogDao.IpAddress, loginLog.IpAddress);
            Assert.Equal(loginLogDao.UserAgent, loginLog.UserAgent);
            Assert.Equal(loginLogDao.LoginTime, loginLog.LoginTime);
            Assert.Equal(loginLogDao.IsSuccessful, loginLog.IsSuccessful);
            Assert.Equal(loginLogDao.FailureReason, loginLog.FailureReason);
        }

        [Fact]
        public void LoginLog_To_LoginLogDao_MapsCorrectly()
        {
            // Arrange
            var loginLog = new LoginLog
            {
                Id = 200,
                AccountId = 2,
                IpAddress = "10.0.0.1",
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)",
                LoginTime = DateTime.UtcNow,
                IsSuccessful = false,
                FailureReason = "Invalid credentials"
            };

            // Act
            var loginLogDao = _mapper.Map<LoginLogDao>(loginLog);

            // Assert
            Assert.NotNull(loginLogDao);
            Assert.Equal(loginLog.Id, loginLogDao.Id);
            Assert.Equal(loginLog.AccountId, loginLogDao.AccountId);
            Assert.Equal(loginLog.IpAddress, loginLogDao.IpAddress);
            Assert.Equal(loginLog.UserAgent, loginLogDao.UserAgent);
            Assert.Equal(loginLog.LoginTime, loginLogDao.LoginTime);
            Assert.Equal(loginLog.IsSuccessful, loginLogDao.IsSuccessful);
            Assert.Equal(loginLog.FailureReason, loginLogDao.FailureReason);
        }

        [Fact]
        public void PracticeLog_With_EmptyCourseId_FiltersInvalidCourseIds()
        {
            // Arrange
            var practiceLog = new PracticeLog
            {
                Id = 1,
                CurrentCourseId = Guid.Empty, // Empty GUID
                CurrentLessonId = 5,
                PracticeStats = new List<DrillStats>
                {
                    new DrillStats
                    {
                        Id = 1,
                        PracticeLogId = 1,
                        CourseId = Guid.Empty, // Invalid course ID
                        Wpm = 70,
                        Accuracy = 93.5,
                        Type = TrainingType.Course,
                        PracticeText = "test text",
                        TypedText = "typed text"
                    },
                    new DrillStats
                    {
                        Id = 2,
                        PracticeLogId = 1,
                        CourseId = Guid.NewGuid(), // Valid course ID
                        Wpm = 75,
                        Accuracy = 95.0,
                        Type = TrainingType.Course,
                        PracticeText = "another test",
                        TypedText = "another typed"
                    }
                },
                PracticeDuration = 3600
            };

            // Act
            var practiceLogDao = _mapper.Map<PracticeLogDao>(practiceLog);

            // Assert
            Assert.NotNull(practiceLogDao);

            // The CurrentCourseId should not be mapped since it's empty
            Assert.Equal(Guid.Empty, practiceLogDao.CurrentCourseId);

            // Only one drill stat should be mapped (the one with valid course ID)
            Assert.Single(practiceLogDao.PracticeStats);
            Assert.NotEqual(Guid.Empty, practiceLogDao.PracticeStats.First().CourseId);
        }

        [Fact]
        public void DrillStats_With_EmptyCourseId_IsNotMapped()
        {
            // Arrange
            var drillStats = new DrillStats
            {
                Id = 1,
                PracticeLogId = 1,
                CourseId = Guid.Empty, // Empty GUID
                Wpm = 70,
                Accuracy = 93.5,
                Type = TrainingType.Course,
                PracticeText = "test text",
                TypedText = "typed text"
            };

            // Act
            var drillStatsDao = _mapper.Map<DrillStatsDao>(drillStats);

            // Assert
            Assert.NotNull(drillStatsDao);

            // The CourseId property should default to empty GUID 
            // since the PreCondition prevents mapping the source value
            Assert.Equal(Guid.Empty, drillStatsDao.CourseId);
        }

        [Fact]
        public void Account_With_InvalidCourseIdsInHistory_FiltersInvalidCourseIds()
        {
            // Arrange
            var validCourseId = Guid.NewGuid();
            var account = new Account
            {
                Id = 1,
                AccountName = "Test Account",
                AccountEmail = "test@example.com",
                History = new PracticeLog
                {
                    Id = 1,
                    CurrentCourseId = Guid.Empty, // Invalid course ID
                    CurrentLessonId = 5,
                    PracticeStats = new List<DrillStats>
                    {
                        new DrillStats
                        {
                            Id = 1,
                            PracticeLogId = 1,
                            CourseId = Guid.Empty, // Invalid course ID
                            Wpm = 70,
                            Accuracy = 93.5,
                            Type = TrainingType.Course
                        },
                        new DrillStats
                        {
                            Id = 2,
                            PracticeLogId = 1,
                            CourseId = validCourseId, // Valid course ID
                            Wpm = 75,
                            Accuracy = 95.0,
                            Type = TrainingType.Course
                        }
                    }
                }
            };

            // Act
            var accountDao = _mapper.Map<AccountDao>(account);

            // Assert
            Assert.NotNull(accountDao);
            Assert.NotNull(accountDao.History);

            // The history's current course ID should be empty
            Assert.Equal(Guid.Empty, accountDao.History.CurrentCourseId);

            // Only the drill stats with valid course ID should be mapped
            Assert.Single(accountDao.History.PracticeStats);
            Assert.Equal(validCourseId, accountDao.History.PracticeStats.First().CourseId);
        }
    }
}