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
                Materials =
               [
                   new CourseMaterial
                    { PracticeText = "Beginner Course.", Level = SkillLevel.Beginner },
                new CourseMaterial
                    { PracticeText = "Novice Course.", Level = SkillLevel.Novice },
                new CourseMaterial
                    { PracticeText = "Intermediate Course.", Level = SkillLevel.Intermediate },
                new CourseMaterial
                    { PracticeText = "Advanced Course.", Level = SkillLevel.Advanced },
                new CourseMaterial
                    { PracticeText = "Expert Course.", Level = SkillLevel.Expert }
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
                    CurrentStats = new TypingStats { Wpm = 20, Accuracy = 70 },
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
                    CurrentStats = new TypingStats { Wpm = 32, Accuracy = 82 },
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
                    CurrentStats = new TypingStats { Wpm = 46, Accuracy = 86 },
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
                    CurrentStats = new TypingStats { Wpm = 61, Accuracy = 94 },
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
        [InlineData(SkillLevel.Beginner, "Beginner Course.")]
        [InlineData(SkillLevel.Novice, "Novice Course.")]
        [InlineData(SkillLevel.Intermediate, "Intermediate Course.")]
        [InlineData(SkillLevel.Advanced, "Advanced Course.")]
        [InlineData(SkillLevel.Expert, "Expert Course.")]
        public void CanGetTextBasedOnLevel(SkillLevel level, string expectedText)
        {
            // Act
            var actualText = _typingTrainer.GetPracticeText(level);

            // Assert
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData("BeginnerUser", "Beginner Course.")]
        [InlineData("NoviceUser", "Novice Course.")]
        [InlineData("IntermediateUser", "Intermediate Course.")]
        [InlineData("AdvancedUser", "Advanced Course.")]
        public void CanGetTextBasedOnAccount(string accountName, string expectedText)
        {
            // Arrange
            var account = _accounts.FirstOrDefault(x => x.AccountName == accountName);
            Assert.NotNull(account);
            _typingTrainer = new TypingTrainer(account);

            // Act
            var actualText = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal(expectedText, actualText);
        }
    }
}