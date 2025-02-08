using TypingMaster.Business;
using TypingMaster.Business.Models;

namespace TypingMaster.Tests
{
    public class AccountServiceTests
    {
        private readonly List<Account> _accounts;
        private TypingTrainer _typingTrainer;

        public AccountServiceTests()
        {
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
            var (_, actualText) = _typingTrainer.GetPracticeText();

            // Assert
            Assert.Equal(expectedText, actualText);
        }
    }
}