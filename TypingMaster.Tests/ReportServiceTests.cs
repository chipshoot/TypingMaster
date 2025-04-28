using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Tests
{
    public class ReportServiceTests
    {
        private readonly Mock<IPracticeLogService> _mockPracticeLogService;
        private readonly Mock<ILogger> _mockLogger;
        private readonly ReportService _reportService;

        //todo: update test class to meet the new design of the ReportService class
        public ReportServiceTests()
        {
            _mockPracticeLogService = new Mock<IPracticeLogService>();
            _mockLogger = new Mock<ILogger>();
            _reportService = new ReportService(_mockPracticeLogService.Object, _mockLogger.Object);
        }

        [Fact]
        public void GetKeyLabels_ReturnsKeyLabels()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    {
                        'a',
                        new KeyStats
                            { Key = "a", TypingCount = 50, CorrectCount = 45, PressDuration = 222, Latency = 33 }
                    },
                    {
                        'b',
                        new KeyStats
                            { Key = "b", TypingCount = 30, CorrectCount = 45, PressDuration = 212, Latency = 35 }
                    },
                }
            };

            // Act
            var result = _reportService.GetKeyLabels(history);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains("a", result);
            Assert.Contains("b", result);
        }

        [Fact]
        public void GetKeyWpm_ReturnsKeyWpm()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    { 'a', new KeyStats { Accuracy = 90.0 } },
                    { 'b', new KeyStats { Accuracy = 85.0 } }
                }
            };

            // Act
            var result = _reportService.GetKeyStats(history);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(2, result["wpm"].Count());
            Assert.Contains(90.0, result["wpm"]);
            Assert.Contains(85.0, result["wpm"]);
        }

        [Fact]
        public void GetKeyLabels_ReturnsEmptyList_WhenAccountNotFound()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    { 'a', new KeyStats { Accuracy = 90.0 } },
                    { 'b', new KeyStats { Accuracy = 85.0 } }
                }
            };

            // Act
            var result = _reportService.GetKeyLabels(history);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetKeyWpm_ReturnsEmptyDictionary_WhenAccountNotFound()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    { 'a', new KeyStats { Accuracy = 90.0 } },
                    { 'b', new KeyStats { Accuracy = 85.0 } }
                }
            };

            // Act
            var result = _reportService.GetKeyStats(history);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}