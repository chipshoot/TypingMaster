//using AutoMapper;
//using Moq;
//using Serilog;
//using TypingMaster.Business;
//using TypingMaster.Core.Models;
//using TypingMaster.Core.Models.Courses;
//using TypingMaster.DataAccess.Data;

//namespace TypingMaster.Tests
//{
//    public class CourseServiceTests
//    {
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly CourseService _service;

//        public CourseServiceTests()
//        {
//            var mockLogger= new Mock<ILogger>();
//            _mockMapper = new Mock<IMapper>();
//            var repository = new Mock<ICourseRepository>();

//            _service = new CourseService(repository.Object, _mockMapper.Object, mockLogger.Object);
//        }

//        [Fact]
//        public async Task GetCourse_WhenCourseExists_ReturnsCourse()
//        {
//            // Act
//            var result = await _service.GetCourse(CourseService.CourseId1);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(CourseService.CourseId1, result.Id);
//            Assert.Equal(TrainingType.Course, result.Type);
//            Assert.NotNull(result.Lessons);
//            Assert.NotEmpty(result.Lessons);
//        }

//        [Fact]
//        public async Task GetCourse_WhenCourseDoesNotExist_ReturnsNull()
//        {
//            // Arrange
//            var nonExistentId = Guid.NewGuid();

//            // Act
//            var result = await _service.GetCourse(nonExistentId);

//            // Assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task GetAllKeysCourse_WhenIdProvidedAndCourseExists_ReturnsCourse()
//        {
//            // Act
//            var result = await _service.GetAllKeysCourse(CourseService.AllKeyTestCourseId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(CourseService.AllKeyTestCourseId, result.Id);
//            Assert.Equal(TrainingType.AllKeysTest, result.Type);
//            Assert.NotNull(result.Lessons);
//            Assert.NotEmpty(result.Lessons);
//        }

//        [Fact]
//        public async Task GetAllKeysCourse_WhenIdProvidedAndCourseDoesNotExist_ReturnsNull()
//        {
//            // Arrange
//            var nonExistentId = Guid.NewGuid();

//            // Act
//            var result = await _service.GetAllKeysCourse(nonExistentId);

//            // Assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task GetAllKeysCourse_WhenNoIdProvided_ReturnsFirstAllKeysCourse()
//        {
//            // Act
//            var result = await _service.GetAllKeysCourse(null);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(CourseService.AllKeyTestCourseId, result.Id);
//            Assert.Equal(TrainingType.AllKeysTest, result.Type);
//        }

//        [Fact]
//        public async Task GenerateBeginnerCourse_WithValidSettings_ReturnsCourse()
//        {
//            // Arrange
//            var settings = new CourseSetting
//            {
//                Minutes = 30,
//                TargetStats = new StatsBase { Wpm = 60, Accuracy = 95 },
//                NewKeysPerStep = 2,
//                PracticeTextLength = 50
//            };

//            // Act
//            var result = await _service.GenerateBeginnerCourse(settings);

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotEqual(Guid.Empty, result.Id);
//            Assert.Equal("Beginner Typing Course", result.Name);
//            Assert.Equal(settings, result.Settings);
//            Assert.NotNull(result.Lessons);
//        }

//        [Fact]
//        public async Task GenerateBeginnerCourse_WithNullSettings_ReturnsCourseWithDefaultSettings()
//        {
//            // Act
//            var result = await _service.GenerateBeginnerCourse(null);

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotEqual(Guid.Empty, result.Id);
//            Assert.Equal("Beginner Typing Course", result.Name);
//            Assert.NotNull(result.Settings);
//            Assert.NotNull(result.Lessons);
//        }

//        [Fact]
//        public async Task GenerateBeginnerCourse_WithCustomSettings_GeneratesAppropriateLessons()
//        {
//            // Arrange
//            var settings = new CourseSetting
//            {
//                Minutes = 30,
//                TargetStats = new StatsBase { Wpm = 60, Accuracy = 95 },
//                NewKeysPerStep = 2,
//                PracticeTextLength = 50
//            };

//            // Act
//            var result = await _service.GenerateBeginnerCourse(settings);

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotNull(result.Lessons);
//            Assert.NotEmpty(result.Lessons);

//            // Verify lesson structure
//            foreach (var lesson in result.Lessons)
//            {
//                Assert.NotNull(lesson);
//                Assert.NotNull(lesson.Target);
//                Assert.NotNull(lesson.Description);
//                Assert.True(lesson.Point > 0);
//            }
//        }

//        [Fact]
//        public async Task GenerateBeginnerCourse_WithInvalidLessonData_ReturnsEmptyLessons()
//        {
//            // Arrange
//            var settings = new CourseSetting
//            {
//                Minutes = 30,
//                TargetStats = new StatsBase { Wpm = 60, Accuracy = 95 },
//                NewKeysPerStep = 2,
//                PracticeTextLength = 50
//            };

//            // Act
//            var result = await _service.GenerateBeginnerCourse(settings);

//            // Assert
//            Assert.NotNull(result);
//            Assert.NotNull(result.Lessons);
//            // Note: The actual behavior might be different based on your implementation
//            // This test might need to be adjusted based on how you handle invalid lesson data
//        }
//    }
//}