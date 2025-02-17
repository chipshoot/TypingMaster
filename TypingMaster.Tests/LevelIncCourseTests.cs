using TypingMaster.Business;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;

namespace TypingMaster.Tests
{
    public class LevelIncCourseTests
    {
        private const string ExpectDescription = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level.";
        private readonly AdvancedLevelCourse _course = new()
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

        [Theory]
        [InlineData(15, 80, SkillLevel.Beginner)]
        [InlineData(35, 85, SkillLevel.Novice)]
        [InlineData(50, 87, SkillLevel.Intermediate)]
        [InlineData(70, 92, SkillLevel.Advanced)]
        [InlineData(75, 95, SkillLevel.Expert)]
        public void CanGetLevelBasedOnStats(int wpm, double accuracy, SkillLevel expectedLevel)
        {
            // Arrange
            var stats = new DrillStats { Wpm = wpm, Accuracy = accuracy };

            // Act
            var actualLevel = stats.GetSkillLevel();

            // Assert
            Assert.Equal(expectedLevel, actualLevel);
        }

        [Fact]
        public void CanGetTextForNewUser()
        {
            // Arrange
            var stats = new DrillStats { Wpm = 0, Accuracy = 0 };

            // Act
            var text = _course.GetPracticeLesson(stats)?.PracticeText;

            // Assert
            Assert.Equal("Beginner Course.", text);
        }

        [Fact]
        public void CanGetNextTextWithSameLevelWhenCurrentStatsDoesNotImprove()
        {
            // Arrange
            var stats = new DrillStats
            {
                CourseId = 1,
                LessonId = 1,
                Wpm = 20,
                Accuracy = 70
            };

            // Act
            var text = _course.GetPracticeLesson(stats)?.PracticeText;

            // Assert
            Assert.Equal("Beginner Course2.", text);
        }

        [Fact]
        public void CanGetNextTextWithNextLevelWhenCurrentStatsImproved()
        {
            // Arrange
            var stats = new DrillStats
            {
                CourseId = 1,
                LessonId = 6,
                Wpm = 74,
                Accuracy = 90
            };

            // Act
            var text = _course.GetPracticeLesson(stats)?.PracticeText;

            // Assert
            Assert.Equal("Novice Course.", text);
        }

        [Fact]
        public void CanGetCourseFinishTextWhenAllLessonFinished()
        {
            const string expectedText = "Congratulation, You have completed all lessons in this course.";

            // Arrange
            var stats = new DrillStats
            {
                CourseId = 1,
                LessonId = 5,
                Wpm = 74,
                Accuracy = 90
            };

            // Act
            var actualText = _course.GetPracticeLesson(stats)?.PracticeText ?? _course.CompleteText;

            // Assert
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData(1, 5, 74, 90, true)]
        [InlineData(1, 5, 24, 20, false)]
        [InlineData(1, 1, 74, 90, false)]
        public void CanCheckCourseIsCompleted(int courseId, int lessonId, int wpm, int accuracy, bool expectResult)
        {
            // Arrange
            var stats = new DrillStats
            {
                CourseId = courseId,
                LessonId = lessonId,
                Wpm = wpm,
                Accuracy = accuracy
            };

            // Act
            var isCompleted = _course.IsCompleted(stats);

            // Assert
            Assert.Equal(expectResult, isCompleted);
        }

        [Fact]
        public void CanGetDescription()
        {
            // Arrange
            var course = new AdvancedLevelCourse();

            // Act
            var description = course.Description;

            // Assert
            Assert.Equal(ExpectDescription, description);
        }
    }
}