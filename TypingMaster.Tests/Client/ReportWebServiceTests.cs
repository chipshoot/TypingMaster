using Moq;
using TypingMaster.Client;
using TypingMaster.Client.Services;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Client
{
    public class ReportWebServiceTests
    {
        private readonly ReportWebService _reportWebService;
        private readonly Mock<IApiConfiguration> _mockApiConfig;
        private readonly HttpClient _httpClient;

        public ReportWebServiceTests()
        {
            _httpClient = new HttpClient();
            _mockApiConfig = new Mock<IApiConfiguration>();
            _mockApiConfig.Setup(x => x.BuildApiUrl(It.IsAny<string>())).Returns<string>(s => s);
            
            _reportWebService = new ReportWebService(_httpClient, _mockApiConfig.Object);
        }

        [Fact]
        public async Task GetKeyLabels_ReturnsKeyLabels_WhenHistoryContainsKeyStats()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    { 'a', new KeyStats { Key = "a", TypingCount = 50, CorrectCount = 45 } },
                    { 'b', new KeyStats { Key = "b", TypingCount = 30, CorrectCount = 25 } }
                }
            };

            // Act
            var result = await _reportWebService.GetKeyLabels(history);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains("a", result);
            Assert.Contains("b", result);
        }

        [Fact]
        public async Task GetKeyLabels_ReturnsEmptyCollection_WhenHistoryHasNoKeyStats()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>()
            };

            // Act
            var result = await _reportWebService.GetKeyLabels(history);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetKeyStats_ReturnsCorrectStats_WhenHistoryContainsKeyStats()
        {
            // Arrange
            var history = new PracticeLog
            {
                KeyStats = new Dictionary<char, KeyStats>
                {
                    { 
                        'a', 
                        new KeyStats { 
                            Key = "a", 
                            TypingCount = 100, 
                            CorrectCount = 90, 
                            PressDuration = 900, 
                            Latency = 450 
                        } 
                    },
                    { 
                        'b', 
                        new KeyStats { 
                            Key = "b", 
                            TypingCount = 80, 
                            CorrectCount = 60, 
                            PressDuration = 600, 
                            Latency = 300 
                        } 
                    }
                }
            };

            // Act
            var result = await _reportWebService.GetKeyStats(history, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            
            // Check typeSpeed (PressDuration / CorrectCount)
            Assert.Equal(10.0, result["typeSpeed"].ElementAt(0), 2); // 900/90 = 10
            Assert.Equal(10.0, result["typeSpeed"].ElementAt(1), 2); // 600/60 = 10
            
            // Check latency (Latency / CorrectCount)
            Assert.Equal(5.0, result["latency"].ElementAt(0), 2); // 450/90 = 5
            Assert.Equal(5.0, result["latency"].ElementAt(1), 2); // 300/60 = 5
            
            // Check accuracy (CorrectCount / TypingCount * 100)
            Assert.Equal(90.0, result["accuracy"].ElementAt(0), 2); // 90/100 * 100 = 90%
            Assert.Equal(75.0, result["accuracy"].ElementAt(1), 2); // 60/80 * 100 = 75%
        }

        [Fact]
        public async Task GetProgressRecords_ReturnsFilteredRecords_ForSpecificTrainingType()
        {
            // Arrange
            var now = DateTime.Now;
            var history = new PracticeLog
            {
                PracticeStats = new List<DrillStats>
                {
                    new DrillStats
                    {
                        Type = TrainingType.Course,
                        Wpm = 60,
                        Accuracy = 95.0,
                        StartTime = now,
                        KeyEvents = new Queue<KeyEvent>(new[]
                        {
                            new KeyEvent { Key = 'a', IsCorrect = true, KeyDownTime = now, KeyUpTime = now.AddMilliseconds(100) },
                            new KeyEvent { Key = 'b', IsCorrect = true, KeyDownTime = now.AddMilliseconds(200), KeyUpTime = now.AddMilliseconds(300) }
                        })
                    },
                    new DrillStats
                    {
                        Type = TrainingType.AllKeysTest,
                        Wpm = 70,
                        Accuracy = 85.0,
                        StartTime = now.AddHours(1),
                        KeyEvents = new Queue<KeyEvent>()
                    }
                }
            };

            var course = new TestCourse
            {
                Id = Guid.NewGuid(),
                Name = "Test Course"
            };

            // Act
            var result = await _reportWebService.GetProgressRecords(history, course, TrainingType.Course);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var record = result.First();
            Assert.Equal("Course", record.Type);
            Assert.Equal("Test Course", record.Name);
            Assert.Equal(95.0, record.OverallAccuracy);
            Assert.Equal(60, record.OverallSpeed);
        }

        // Helper test class
        private class TestCourse : CourseBase
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public override Lesson? GetPracticeLesson(int curLessonId, StatsBase stats)
            {
                throw new NotImplementedException();
            }

            public override bool IsCompleted(int curLessonId, StatsBase stats)
            {
                throw new NotImplementedException();
            }
        }
    }
}
