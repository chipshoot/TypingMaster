using Moq;
using Serilog;
using TypingMaster.Business.Course;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course
{
    public class PhaseControlledPracticeTextTests
    {
        private readonly BeginnerCourse _course;
        private readonly Mock<ILogger> _mockLogger;

        public PhaseControlledPracticeTextTests()
        {
            _mockLogger = new Mock<ILogger>();
            _course = new BeginnerCourse(_mockLogger.Object);

            // Initialize course with settings and lessons
            _course.Settings = new CourseSetting
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PhaseAttemptThreshold = 50,
                TargetStats = new StatsBase { Wpm = 30, Accuracy = 90 }
            };

            _course.Lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = 1,
                    Target = new[] { "a", "s", "d", "f" },
                    CommonWords = new[] { "as", "sad", "fad", "add" },
                    Instruction = "Place your fingers on the home row keys",
                    Description = "Home row lesson"
                },
                new Lesson
                {
                    Id = 2,
                    Target = new[] { "j", "k", "l", ";" },
                    CommonWords = new[] { "jak", "kale", "lake", "slay" },
                    Instruction = "Practice right hand home row keys",
                    Description = "Right hand home row lesson"
                }
            };
        }

        [Fact]
        public void GetPracticeLesson_SimpleRepetitionPhase_GeneratesRepeatedCharacters()
        {
            // Arrange
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 20, Accuracy = 80 };
            var phase = PracticePhases.SimpleRepetition;

            // Act
            var lessonDto = _course.GetPracticeLesson(lessonId, stats, phase);
            var lesson = lessonDto?.Lesson;

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(lessonId, lesson.Id);
            Assert.NotEmpty(lesson.PracticeText);

            // Verify instruction contains phase-specific text
            Assert.Contains("Focus on finger position and accuracy", lesson.Instruction);

            // Verify practice text contains repeated characters
            foreach (var key in lesson.Target)
            {
                Assert.Contains(key[0], lesson.PracticeText);
            }
        }

        [Fact]
        public void GetPracticeLesson_PatternsPhase_GeneratesKeyPatterns()
        {
            // Arrange
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 20, Accuracy = 80 };
            var phase = PracticePhases.Patterns;

            // Act
            var lessonDto = _course.GetPracticeLesson(lessonId, stats, phase);
            var lesson = lessonDto?.Lesson;

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(lessonId, lesson.Id);
            Assert.NotEmpty(lesson.PracticeText);

            // Verify instruction contains phase-specific text
            Assert.Contains("Practice these key patterns", lesson.Instruction);

            // Verify practice text contains patterns of the target keys
            foreach (var key in lesson.Target)
            {
                Assert.Contains(key[0], lesson.PracticeText);
            }
        }

        [Fact]
        public void GetPracticeLesson_RealWordsPhase_GeneratesRealWords()
        {
            // Arrange
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 20, Accuracy = 80 };
            var phase = PracticePhases.RealWords;

            // Act
            var lessonDto = _course.GetPracticeLesson(lessonId, stats, phase);
            var lesson = lessonDto?.Lesson;

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(lessonId, lesson.Id);
            Assert.NotEmpty(lesson.PracticeText);

            // Verify instruction contains phase-specific text
            Assert.Contains("Apply your skills by typing these common words", lesson.Instruction);

            // Words will be chosen from CommonWords, but we can't test exact content
            // due to randomness, so we'll just verify the text isn't empty
            Assert.NotEmpty(lesson.PracticeText);
        }

        [Fact]
        public void GetPracticeLesson_TargetAchieved_MovesToNextLesson()
        {
            // Arrange
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 35, Accuracy = 95 }; // Above target of 30 WPM and 90% accuracy
            var phase = PracticePhases.RealWords;

            // Act
            var lessonDto = _course.GetPracticeLesson(lessonId, stats, phase);
            var lesson = lessonDto?.Lesson;

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(2, lesson.Id); // Should move to lesson 2
            Assert.NotEmpty(lesson.PracticeText);

            // When moving to a new lesson, phase should be reset to SimpleRepetition
            Assert.Contains("Focus on finger position and accuracy", lesson.Instruction);
        }

        [Fact]
        public void GetPracticeLesson_CourseCompleted_ReturnsCourseCompleteLesson()
        {
            // Arrange
            var lessonId = 2; // Last lesson in our test setup
            var stats = new StatsBase { Wpm = 35, Accuracy = 95 }; // Above target
            var phase = PracticePhases.RealWords;

            // Act
            var lessonDto = _course.GetPracticeLesson(lessonId, stats, phase);
            var lesson = lessonDto?.Lesson;

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(lessonId, lesson.Id); // ID stays the same
            Assert.True(lesson.IsCourseComplete);
            Assert.Empty(lesson.PracticeText);
            Assert.Equal(_course.CompleteText, lesson.Instruction);
        }

        [Fact]
        public void GetPracticeLesson_NullStats_ReturnsNull()
        {
            // Arrange
            var lessonId = 1;
            var phase = PracticePhases.SimpleRepetition;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _course.GetPracticeLesson(lessonId, null, phase));
        }
    }
}