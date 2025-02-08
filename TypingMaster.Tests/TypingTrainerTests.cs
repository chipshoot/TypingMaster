using TypingMaster.Business;
using TypingMaster.Business.Models;

namespace TypingMaster.Tests
{
    public class TypingTrainerTests
    {
        private readonly List<Account> _accounts;
        private TypingTrainer _typingTrainer;

        public TypingTrainerTests()
        {
            var course = new Course
            {
                Id = 1,
                Lessons =
                [
                    new Lesson
                        { Id = 1, PracticeText = "Beginner Course.", Point = 1 },
                    new Lesson
                        { Id = 2, PracticeText = "Novice Course.", Point = 2 },
                    new Lesson
                        { Id = 3, PracticeText = "Intermediate Course.", Point = 3 },
                    new Lesson
                        { Id = 4, PracticeText = "Advanced Course.", Point = 4 },
                    new Lesson
                        { Id = 5, PracticeText = "Expert Course.", Point = 5 },
                    new Lesson
                        { Id = 6, PracticeText = "Beginner Course2.", Point = 1 }
                ]
            };

            _accounts =
            [
                new Account
                {
                    Id = 1,
                    AccountName = "BeginnerUser",
                    Email = "testuser@example.com",
                    User = new UserProfile { FirstName = "Beginner", LastName = "User", Title = "Mr." },
                    GoalStats = new TypingStats { Wpm = 75, Accuracy = 95 },
                    Progress =
                    [
                        new LearningProgress
                            { Stats = new TypingStats { Wpm = 20, Accuracy = 70 }, CourseId = 1, LessonId = 1 }
                    ],
                    PracticeTime = 10.5,
                    CurrentCourse = course
                },

                new Account
                {
                    Id = 2,
                    AccountName = "NoviceUser",
                    Email = "testuser@example.com",
                    User = new UserProfile { FirstName = "Novice", LastName = "User", Title = "Mr." },
                    GoalStats = new TypingStats { Wpm = 75, Accuracy = 95 },
                    Progress =
                    [
                        new LearningProgress
                            { Stats = new TypingStats { Wpm = 32, Accuracy = 82 }, CourseId = 1, LessonId = 2 }
                    ],
                    PracticeTime = 10.5,
                    CurrentCourse = course
                },

                new Account
                {
                    Id = 3,
                    AccountName = "IntermediateUser",
                    Email = "testuser@example.com",
                    User = new UserProfile { FirstName = "Intermediate", LastName = "User", Title = "Mr." },
                    GoalStats = new TypingStats { Wpm = 75, Accuracy = 95 },
                    Progress =
                    [
                        new LearningProgress
                            { Stats = new TypingStats { Wpm = 46, Accuracy = 86 }, CourseId = 1, LessonId = 3 }
                    ],
                    PracticeTime = 10.5,
                    CurrentCourse = course
                },

                new Account
                {
                    Id = 4,
                    AccountName = "AdvancedUser",
                    Email = "testuser@example.com",
                    User = new UserProfile { FirstName = "Advanced", LastName = "User", Title = "Miss" },
                    GoalStats = new TypingStats { Wpm = 80, Accuracy = 95 },
                    Progress =
                    [
                        new LearningProgress
                            { Stats = new TypingStats { Wpm = 61, Accuracy = 94 }, CourseId = 1, LessonId = 4 }
                    ],
                    PracticeTime = 10.5,
                    CurrentCourse = course
                }
            ];
            _typingTrainer = new TypingTrainer(_accounts[0]);
        }

        [Theory]
        [InlineData(15, 80, SkillLevel.Beginner)]
        [InlineData(35, 85, SkillLevel.Novice)]
        [InlineData(50, 87, SkillLevel.Intermediate)]
        [InlineData(70, 92, SkillLevel.Advanced)]
        [InlineData(75, 95, SkillLevel.Expert)]
        public void CanGetLevelBasedOnStats(int wpm, double accuracy, SkillLevel expectedLevel)
        {
            // Arrange
            var stats = new TypingStats { Wpm = wpm, Accuracy = accuracy };

            // Act
            var actualLevel = _typingTrainer.GetSkillLevel(stats);

            // Assert
            Assert.Equal(expectedLevel, actualLevel);
        }

        [Theory]
        [InlineData(1, "Beginner Course.")]
        [InlineData(2, "Novice Course.")]
        [InlineData(3, "Intermediate Course.")]
        [InlineData(4, "Advanced Course.")]
        [InlineData(5, "Expert Course.")]
        public void CanGetTextBasedOnId(int lessonId, string expectedText)
        {
            // Act
            var actualText = _typingTrainer.GetPracticeText(lessonId);

            // Assert
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData("BeginnerUser", "Beginner Course2.")]
        [InlineData("NoviceUser", "Novice Course.")]
        [InlineData("IntermediateUser", "Intermediate Course.")]
        [InlineData("AdvancedUser", "Expert Course.")]
        public void CanGetTextBasedOnAccount(string accountName, string expectedText)
        {
            // Arrange
            var account = _accounts.FirstOrDefault(x => x.AccountName == accountName);
            Assert.NotNull(account);
            _typingTrainer = new TypingTrainer(account);

            // Act
            var (_, actualText) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal(expectedText, actualText);
        }

        [Fact]
        public void CanGetTextForNewUser()
        {
            // Arrange
            var account = _accounts.FirstOrDefault();
            Assert.NotNull(account);
            account.Progress = new List<LearningProgress>();

            // Act
            var (_, text) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal("Beginner Course.", text);
        }

        [Fact]
        public void CanGetNextTextWithSameLevelWhenCurrentStatsDoesNotImprove()
        {
            // Arrange
            var account = _accounts.FirstOrDefault();
            Assert.NotNull(account);
            account.Progress =
            [
                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new TypingStats { Wpm = 20, Accuracy = 70 } }
            ];

            // Act
            var (_, text) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal("Beginner Course2.", text);
        }

        [Fact]
        public void CanGetNextTextWithNextLevelWhenCurrentStatsImproved()
        {
            // Arrange
            var account = _accounts.FirstOrDefault();
            Assert.NotNull(account);
            account.Progress =
            [
                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new TypingStats { Wpm = 20, Accuracy = 70 } },
                new LearningProgress { CourseId = 1, LessonId = 6, Stats = new TypingStats { Wpm = 74, Accuracy = 90 } }
            ];

            // Act
            var (_, text) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal("Novice Course.", text);
        }

        [Fact]
        public void CanGetCourseFinishTextWhenAllLessonFinished()
        {
            // Arrange
            const string expectedText = "Congratulation, You have completed all lessons in this course.";
            var account = _accounts.FirstOrDefault();
            Assert.NotNull(account);
            account.Progress =
            [
                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new TypingStats { Wpm = 20, Accuracy = 70 } },
                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new TypingStats { Wpm = 74, Accuracy = 90 } }
            ];

            _typingTrainer = new TypingTrainer(account);

            // Act
            var (_, actualText) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal(expectedText, actualText);
        }
    }
}