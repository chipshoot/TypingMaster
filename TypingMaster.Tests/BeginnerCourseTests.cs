using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;
using TypingMaster.Tests.Utility;

namespace TypingMaster.Tests
{
    public class BeginnerCourseTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private const int PracticeTextLength = 30;
        private static BeginnerCourse _course;
        private static bool _initialized = false;
        private static readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);

        //todo: update test class to meet the new design of the ReportService class
        public BeginnerCourseTests()
        {
            _mockLogger = new Mock<ILogger>();
            InitializeAsync().GetAwaiter().GetResult();
        }
        private async Task InitializeAsync()
        {
            if (_initialized)
                return;

            await _initializationLock.WaitAsync();
            try
            {
                if (!_initialized)
                {
                    var settings = new CourseSetting
                    {
                        Minutes = 120,
                        NewKeysPerStep = 1,
                        TargetStats = new StatsBase
                        {
                            Accuracy = 85,
                            Wpm = 30
                        },
                        PracticeTextLength = PracticeTextLength
                    };

                    var courseService = new CourseService(_mockLogger.Object);
                    _course = await courseService.GenerateBeginnerCourse(settings) as BeginnerCourse;
                    _initialized = true;
                }
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        [Fact]
        public void HasNoEmptyId()
        {
            // Arrange
            // Act
            var id = _course.Id;

            // Assert
            Assert.NotEqual(Guid.Empty, id);

        }

        [Fact]
        public void HasNoEmptyName()
        {
            // Arrange
            // Act
            var name = _course.Name;

            // Assert
            Assert.Equal("Beginner Typing Course", name);
        }

        [Fact]
        public void HasPracticeType()
        {
            // Arrange
            // Act
            var type = _course.Type;

            // Assert
            Assert.Equal(TrainingType.Course, type);
        }

        [Fact]
        public void HasCourseDescription()
        {
            // Arrange
            // Act
            var description = _course.Description;

            // Assert
            Assert.NotEmpty(description);
        }

        [Fact]
        public void GetLeftHomeKeyTestWords()
        {
            // Arrange
            var stats = new StatsBase
            {
                Accuracy = 50,
                Wpm = 20,
            };

            // Act
            var lesson = _course.GetPracticeLesson(1, stats);

            // Assert
            Assert.NotNull(lesson);
            Assert.NotEmpty(lesson.PracticeText);
            Assert.True(PracticeTextLength >= lesson.PracticeText.Length);
            Assert.True(lesson.PracticeText.ContainsOnlyAllowedChars(['a', 's', 'd', 'f']));
        }

        [Fact]
        public void GeneratePracticeText_RespectsLengthLimit()
        {
            // Arrange
            var stats = new StatsBase
            {
                Accuracy = 50,
                Wpm = 20,
            };

            // Act
            var lesson = _course.GetPracticeLesson(1, stats);

            // Assert
            Assert.NotNull(lesson);
            Assert.NotEmpty(lesson.PracticeText);
            Assert.True(lesson.PracticeText.Length <= _course.Settings.PracticeTextLength, 
                $"Practice text length {lesson.PracticeText.Length} should be less than or equal to {_course.Settings.PracticeTextLength}");
        }

        [Fact]
        public void GeneratePracticeText_DifferentTextsForSameLesson()
        {
            // Arrange
            var stats = new StatsBase
            {
                Accuracy = 50,
                Wpm = 20,
            };

            // Act - get practice text twice for the same lesson
            var lesson1 = _course.GetPracticeLesson(1, stats);
            var lesson2 = _course.GetPracticeLesson(1, stats);

            // Assert - texts should be different due to randomization
            Assert.NotNull(lesson1);
            Assert.NotNull(lesson2);
            Assert.NotEmpty(lesson1.PracticeText);
            Assert.NotEmpty(lesson2.PracticeText);
            Assert.NotEqual(lesson1.PracticeText, lesson2.PracticeText);
        }

        [Fact]
        public void GeneratePracticeText_HandlesMultipleLessons()
        {
            // Arrange
            var stats = new StatsBase
            {
                Accuracy = 100, // Perfect stats to move to next lesson
                Wpm = 100,
            };

            // Act - get practice texts for two consecutive lessons
            var lesson1 = _course.GetPracticeLesson(1, stats);
            var lesson2 = _course.GetPracticeLesson(2, stats);

            // Assert - both lessons should have valid practice texts
            Assert.NotNull(lesson1);
            Assert.NotNull(lesson2);
            Assert.NotEmpty(lesson1.PracticeText);
            Assert.NotEmpty(lesson2.PracticeText);
    
            // Lesson texts should differ as they have different target keys
            Assert.NotEqual(lesson1.PracticeText, lesson2.PracticeText);
        }
    }
}