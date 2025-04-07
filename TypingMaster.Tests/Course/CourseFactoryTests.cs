using Moq;
using Serilog;
using TypingMaster.Business.Course;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course
{
    public class CourseFactoryTests
    {
        private readonly CourseFactory _courseFactory;

        public CourseFactoryTests()
        {
            var loggerMock = new Mock<ILogger>();
            _courseFactory = new CourseFactory(loggerMock.Object);

            // Clear cache before each test
            CourseFactory.ClearLessonCache();
        }

        [Fact]
        public void CreateCourseInstance_BeginnerCourse_ReturnsBeginnerCourseInstance()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.BeginnerCourseName,
                Type = TrainingType.Course,
                Settings = new CourseSetting()
            };

            // Act
            var result = _courseFactory.CreateCourseInstance(courseDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<BeginnerCourse>(result);
            Assert.Equal(courseDto.Id, result.Id);
            Assert.Equal(courseDto.Name, result.Name);
            Assert.Equal(courseDto.Settings, result.Settings);
        }

        [Fact]
        public void CreateCourseInstance_AdvancedLevelCourse_ReturnsAdvancedLevelCourseInstance()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.AdvancedLevelCourseName,
                Type = TrainingType.Course,
                Settings = new CourseSetting()
            };

            // Act
            var result = _courseFactory.CreateCourseInstance(courseDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<AdvancedLevelCourse>(result);
            Assert.Equal(courseDto.Id, result.Id);
            Assert.Equal(courseDto.Name, result.Name);
            Assert.Equal(courseDto.Settings, result.Settings);
        }

        [Fact]
        public void CreateCourseInstance_AllKeysCourse_ReturnsPracticeCourseInstance()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.AllKeysCourseName,
                Type = TrainingType.AllKeysTest,
                Settings = new CourseSetting(),
                LessonDataUrl = "testurl"
            };

            // Act
            var result = _courseFactory.CreateCourseInstance(courseDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<PracticeCourse>(result);
            Assert.Equal(courseDto.Id, result.Id);
            Assert.Equal(courseDto.Name, result.Name);
            Assert.Equal(courseDto.Settings, result.Settings);
        }

        [Fact]
        public void CreateCourseInstance_SpeedTestCourse_ReturnsPracticeCourseInstance()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.SpeedTestCourseName,
                Type = TrainingType.SpeedTest,
                Settings = new CourseSetting(),
                LessonDataUrl = "testurl"
            };

            // Act
            var result = _courseFactory.CreateCourseInstance(courseDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<PracticeCourse>(result);
            Assert.Equal(courseDto.Id, result.Id);
            Assert.Equal(courseDto.Name, result.Name);
            Assert.Equal(courseDto.Settings, result.Settings);
        }

        [Fact]
        public void CreateCourseInstance_UnsupportedCourse_ReturnsNull()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = "UnsupportedCourse",
                Type = TrainingType.Course,
                Settings = new CourseSetting()
            };

            // Act
            var result = _courseFactory.CreateCourseInstance(courseDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateCourseInstance_NullDto_ReturnsNull()
        {
            // Act
            var result = _courseFactory.CreateCourseInstance(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetInitializedCourse_ValidCourse_ReturnsInitializedCourse()
        {
            // This test requires setting up a temporary lesson data file
            // You might want to use a test-specific file or mock the file system
            // For now, we'll just verify the method returns a course instance
            var testDataDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
            var testFilePath = Path.Combine(testDataDir, "test-beginner-lessons.json");

            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.BeginnerCourseName,
                Type = TrainingType.Course,
                Settings = new CourseSetting(),
                LessonDataUrl = testFilePath,
                Description = TypingMasterConstants.BeginnerCourseDescription
            };

            try
            {
                // Act
                var result = _courseFactory.GetInitializedCourse(courseDto);

                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<BeginnerCourse>(result);
        
                // Verify lessons were loaded
                Assert.NotNull(result.Lessons);
                Assert.Equal(35, result.Lessons.Count());
        
                // Verify lesson content matches our test data
                var lessons = result.Lessons.ToList();
                Assert.Equal(1, lessons[0].Id);
                Assert.Equal(["a", "s", "d", "f"], lessons[0].Target);
                Assert.Equal("Learn the left hand home row keys", lessons[0].Description);
                Assert.Equal(1, lessons[0].Point);
        
                Assert.Equal(2, lessons[1].Id);
                Assert.Equal(["a", "s", "d", "f", "j"], lessons[1].Target);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Fact]
        public void GetInitializedCourse_InvalidCourse_ReturnsNull()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = "InvalidCourse",
                Type = TrainingType.Course,
                Settings = new CourseSetting()
            };

            // Act
            var result = _courseFactory.GetInitializedCourse(courseDto);

            // Assert
            Assert.Null(result);
        }
    }
}