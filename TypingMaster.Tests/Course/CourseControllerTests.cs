using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Core.Utility;
using TypingMaster.DataAccess.Data;
using TypingMaster.Server.Controllers;

namespace TypingMaster.Tests.Course
{
    public class CourseControllerTests
    {
        private readonly Mock<CourseService> _mockCourseService;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            // Create mock logger first
            var mockLogger = new Mock<ILogger>();

            // Create a mock of the CourseService directly (not just the interface)
            _mockCourseService = new Mock<CourseService>(
                mockLogger.Object,
                Mock.Of<ICourseRepository>(),
                Mock.Of<IAccountRepository>(),
                Mock.Of<IMapper>(),
                Mock.Of<IConfiguration>());

            // Set up ProcessResult - since we're mocking the concrete class, we access it directly
            var processResult = new ProcessResult();
            //_mockCourseService.Setup(s => s.ProcessResult).Returns(processResult);

            _controller = new CourseController(_mockCourseService.Object);
        }

        #region GetCourse Tests

        [Fact]
        public async Task GetCourse_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = TypingMasterConstants.BeginnerCourseName,
                Type = TrainingType.Course
            };

            _mockCourseService.Setup(s => s.GetCourse(courseId))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsAssignableFrom<CourseDto>(okResult.Value);
            Assert.Equal(courseId, returnedCourse.Id);
            Assert.Equal(TypingMasterConstants.BeginnerCourseName, returnedCourse.Name);
        }

        [Fact]
        public async Task GetCourse_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockCourseService
                .Setup(s => s.GetCourse(courseId))
                .ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCourse_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockCourseService
                .Setup(s => s.GetCourse(courseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Error retrieving course", badRequestResult.Value.ToString());
        }

        #endregion GetCourse Tests

        #region GetCoursesByType Tests

        [Fact]
        public async Task GetCoursesByType_ExistingCourses_ReturnsOkResult()
        {
            // Arrange
            var accountId = 1;
            var type = TrainingType.Course;
            var expectedCourses = new List<CourseDto>
            {
                new() { Id = Guid.NewGuid(), Name = TypingMasterConstants.BeginnerCourseName },
                new() { Id = Guid.NewGuid(), Name = TypingMasterConstants.AdvancedLevelCourseName }
            };

            _mockCourseService.Setup(s => s.GetCoursesByType(accountId, type))
                .ReturnsAsync(expectedCourses);

            // Act
            var result = await _controller.GetCoursesByType(accountId, type);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourses = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value);
            Assert.Equal(2, returnedCourses.Count());
        }

        [Fact]
        public async Task GetCoursesByType_NoCourses_ReturnsNotFound()
        {
            // Arrange
            var accountId = 1;
            var type = TrainingType.Course;
            _mockCourseService.Setup(s => s.GetCoursesByType(accountId, type))
                .ReturnsAsync(new List<CourseDto>());

            // Act
            var result = await _controller.GetCoursesByType(accountId, type);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [InlineData(TrainingType.Course)]
        [InlineData(TrainingType.AllKeysTest)]
        [InlineData(TrainingType.SpeedTest)]
        [InlineData(TrainingType.Game)]
        public async Task GetCoursesByType_DifferentTypes_CallsServiceWithCorrectType(TrainingType type)
        {
            // Arrange
            var accountId = 1;
            _mockCourseService.Setup(s => s.GetCoursesByType(accountId, type))
                .ReturnsAsync(new List<CourseDto> { new() { Type = type } });

            // Act
            await _controller.GetCoursesByType(accountId, type);

            // Assert
            _mockCourseService.Verify(s => s.GetCoursesByType(accountId, type), Times.Once);
        }

        #endregion GetCoursesByType Tests

        #region CreateCourse Tests

        [Fact]
        public async Task CreateCourse_ValidCourse_ReturnsCreatedAtAction()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Name = TypingMasterConstants.BeginnerCourseName,
                Type = TrainingType.Course,
                AccountId = 1
            };

            var createdCourse = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.BeginnerCourseName,
                Type = TrainingType.Course,
                AccountId = 1
            };

            _mockCourseService.Setup(s => s.CreateCourse(It.IsAny<CourseDto>()))
                .ReturnsAsync(createdCourse);

            // Act
            var result = await _controller.CreateCourse(courseDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(CourseController.GetCourse), createdAtActionResult.ActionName);
            Assert.Equal(createdCourse.Id, createdAtActionResult.RouteValues["id"]);

            var returnedCourse = Assert.IsType<CourseDto>(createdAtActionResult.Value);
            Assert.Equal(createdCourse.Id, returnedCourse.Id);
            Assert.Equal(courseDto.Name, returnedCourse.Name);
            Assert.Equal(courseDto.Type, returnedCourse.Type);
            Assert.Equal(courseDto.AccountId, returnedCourse.AccountId);
        }

        [Fact]
        public async Task CreateCourse_NullCourse_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateCourse(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Course data is required", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCourse_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var courseDto = new CourseDto { Name = "Test Course" };
            _mockCourseService.Setup(s => s.CreateCourse(It.IsAny<CourseDto>()))
                .ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.CreateCourse(courseDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Failed to create the course", badRequestResult.Value);
        }

        #endregion CreateCourse Tests

        #region GenerateBeginnerCourse Tests

        [Fact]
        public async Task GenerateBeginnerCourse_ValidSettings_ReturnsOkResult()
        {
            // Arrange
            var settings = new CourseSetting
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PracticeTextLength = 50,
                TargetStats = new StatsBase { Wpm = 30, Accuracy = 90 }
            };

            var expectedCourse = new CourseDto
            {
                Id = Guid.NewGuid(),
                Name = TypingMasterConstants.BeginnerCourseName,
                Settings = settings
            };

            _mockCourseService.Setup(s => s.GenerateBeginnerCourse(settings))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GenerateBeginnerCourse(settings);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal(expectedCourse.Id, returnedCourse.Id);
            Assert.Equal(expectedCourse.Name, returnedCourse.Name);
            Assert.Equal(expectedCourse.Settings, returnedCourse.Settings);
        }

        [Fact]
        public async Task GenerateBeginnerCourse_NullSettings_ReturnsBadRequest()
        {
            // Arrange
            _mockCourseService.Setup(s => s.GenerateBeginnerCourse(null))
                .ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.GenerateBeginnerCourse(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion GenerateBeginnerCourse Tests

        #region GenerateStartStats Tests

        [Fact]
        public async Task GenerateStartStats_ReturnsOkResult()
        {
            // Arrange
            var expectedStats = new DrillStats
            {
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Wpm = 0,
                Accuracy = 0,
                StartTime = DateTime.Now,
                FinishTime = DateTime.Now.AddMinutes(5)
            };

            _mockCourseService.Setup(s => s.GenerateStartStats())
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GenerateStartStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStats = Assert.IsType<DrillStats>(okResult.Value);
            Assert.Equal(expectedStats.CourseId, returnedStats.CourseId);
            Assert.Equal(expectedStats.LessonId, returnedStats.LessonId);
            Assert.Equal(expectedStats.Wpm, returnedStats.Wpm);
            Assert.Equal(expectedStats.Accuracy, returnedStats.Accuracy);
        }

        [Fact]
        public async Task GenerateStartStats_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockCourseService.Setup(s => s.GenerateStartStats())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GenerateStartStats();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion GenerateStartStats Tests

        #region GetPracticeLesson Tests

        [Fact]
        public async Task GetPracticeLesson_ExistingLesson_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 30, Accuracy = 85 };
            var expectedLesson = new Lesson
            {
                Id = lessonId,
                Description = "Practice Lesson",
                Target = new[] { "a", "s", "d", "f" },
                Instruction = "Place your fingers on the home row"
            };

            _mockCourseService.Setup(s => s.GetPracticeLesson(courseId, lessonId, stats))
                .ReturnsAsync(expectedLesson);

            // Act
            var result = await _controller.GetPracticeLesson(courseId, lessonId, stats);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLesson = Assert.IsType<Lesson>(okResult.Value);
            Assert.Equal(lessonId, returnedLesson.Id);
            Assert.Equal(expectedLesson.Description, returnedLesson.Description);
            Assert.Equal(expectedLesson.Target, returnedLesson.Target);
            Assert.Equal(expectedLesson.Instruction, returnedLesson.Instruction);
        }

        [Fact]
        public async Task GetPracticeLesson_NonExistingLesson_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 30, Accuracy = 85 };

            _mockCourseService.Setup(s => s.GetPracticeLesson(courseId, lessonId, stats))
                .ReturnsAsync((Lesson)null);

            // Act
            var result = await _controller.GetPracticeLesson(courseId, lessonId, stats);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPracticeLesson_NullStats_ReturnsBadRequest()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessonId = 1;

            // Act
            var result = await _controller.GetPracticeLesson(courseId, lessonId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion GetPracticeLesson Tests
    }
}