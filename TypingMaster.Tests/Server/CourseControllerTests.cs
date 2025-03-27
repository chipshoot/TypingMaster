using Microsoft.AspNetCore.Mvc;
using Moq;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Contract;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Server.Controllers;

namespace TypingMaster.Tests.Server
{
    public class CourseControllerTests
    {
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            _courseServiceMock = new Mock<ICourseService>();
            _controller = new CourseController(_courseServiceMock.Object);
        }

        [Fact]
        public async Task GetCourse_WhenCourseExists_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new Mock<ICourse>().Object;
            _courseServiceMock.Setup(x => x.GetCourse(courseId))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<Mock<ICourse>>(okResult.Value);
            Assert.Same(expectedCourse, returnedCourse);
        }

        [Fact]
        public async Task GetCourse_WhenCourseDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseServiceMock.Setup(x => x.GetCourse(courseId))
                .ReturnsAsync((ICourse?)null);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllKeysCourse_WhenCourseExists_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new Mock<ICourse>().Object;
            _courseServiceMock.Setup(x => x.GetAllKeysCourse(courseId))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GetAllKeysCourse(courseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<Mock<ICourse>>(okResult.Value);
            Assert.Same(expectedCourse, returnedCourse);
        }

        [Fact]
        public async Task GetAllKeysCourse_WhenCourseDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseServiceMock.Setup(x => x.GetAllKeysCourse(courseId))
                .ReturnsAsync((ICourse?)null);

            // Act
            var result = await _controller.GetAllKeysCourse(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllKeysCourse_WhenNoIdProvided_ReturnsOkResult()
        {
            // Arrange
            var expectedCourse = new Mock<ICourse>().Object;
            _courseServiceMock.Setup(x => x.GetAllKeysCourse(null))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GetAllKeysCourse(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<Mock<ICourse>>(okResult.Value);
            Assert.Same(expectedCourse, returnedCourse);
        }

        [Fact]
        public async Task GenerateBeginnerCourse_ReturnsOkResult()
        {
            // Arrange
            var settings = new CourseSetting();
            var expectedCourse = new Mock<ICourse>().Object;
            _courseServiceMock.Setup(x => x.GenerateBeginnerCourse(settings))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GenerateBeginnerCourse(settings);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<Mock<ICourse>>(okResult.Value);
            Assert.Same(expectedCourse, returnedCourse);
        }

        [Fact]
        public async Task GenerateBeginnerCourse_WithNullSettings_ReturnsOkResult()
        {
            // Arrange
            var expectedCourse = new Mock<ICourse>().Object;
            _courseServiceMock.Setup(x => x.GenerateBeginnerCourse(It.IsAny<CourseSetting>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GenerateBeginnerCourse(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourse = Assert.IsType<Mock<ICourse>>(okResult.Value);
            Assert.Same(expectedCourse, returnedCourse);
        }
    }
}