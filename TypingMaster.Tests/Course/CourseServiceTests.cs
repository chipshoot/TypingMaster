using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests.Course
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger>();

            // Setup mock configuration
            var inMemorySettings = new Dictionary<string, string> {
                {"CourseSettings:DefaultLessonUrls:BeginnerCourse", "TestData/test-beginner-lessons.json"},
                {"CourseSettings:DefaultLessonUrls:AdvancedLevelCourse", "TestData/test-beginner-lessons.json"},
                {"CourseSettings:DefaultLessonUrls:AllKeysTestCourse", "TestData/test-practice-lessons.json"},
                {"CourseSettings:DefaultLessonUrls:SpeedTestCourse", "TestData/test-practice-lessons.json"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _courseService = new CourseService(
                _courseRepositoryMock.Object,
                _accountRepositoryMock.Object,
                _mapperMock.Object,
                loggerMock.Object,
                configuration
            );
        }

        [Fact]
        public async Task GetCourse_WhenCourseExists_ReturnsCourseDtoSuccessfully()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var courseDao = new CourseDao { Id = courseId, Name = TypingMasterConstants.BeginnerCourseName };
            var expectedCourseDto = new CourseDto { Id = courseId, Name = "Test Course" };

            _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseId))
                .ReturnsAsync(courseDao);
            _mapperMock.Setup(x => x.Map<CourseDto>(courseDao))
                .Returns(expectedCourseDto);

            // Act
            var result = await _courseService.GetCourse(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCourseDto);
            _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);
            _mapperMock.Verify(x => x.Map<CourseDto>(courseDao), Times.Once);
        }

        [Fact]
        public async Task GetCourse_WhenCourseDoesNotExist_ReturnsNull()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseId))
                .ReturnsAsync((CourseDao?)null);

            // Act
            var result = await _courseService.GetCourse(courseId);

            // Assert
            result.Should().BeNull();
            _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);
            _mapperMock.Verify(x => x.Map<CourseDto>(It.IsAny<CourseDao>()), Times.Never);
        }

        [Fact]
        public async Task GetCoursesByType_WhenCoursesExist_ReturnsCoursesSuccessfully()
        {
            // Arrange
            var accountId = 1;
            var type = TrainingType.Course;
            var courseDaos = new List<CourseDao>
            {
                new() { Id = Guid.NewGuid(), Name = "Course 1", Type = type.ToString() },
                new() { Id = Guid.NewGuid(), Name = "Course 2", Type = type.ToString() }
            };
            var expectedCourseDtos = courseDaos.Select(c => new CourseDto { Id = c.Id, Name = c.Name, Type = type }).ToList();

            _courseRepositoryMock.Setup(x => x.GetCoursesByTypeAsync(accountId, type))
                .ReturnsAsync(courseDaos);
            _mapperMock.Setup(x => x.Map<IEnumerable<CourseDto>>(courseDaos))
                .Returns(expectedCourseDtos);

            // Act
            var result = await _courseService.GetCoursesByType(accountId, type);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCourseDtos);
            _courseRepositoryMock.Verify(x => x.GetCoursesByTypeAsync(accountId, type), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<CourseDto>>(courseDaos), Times.Once);
        }

        [Fact]
        public async Task GetPracticeCoursesByType_WhenCoursesNotExist_ReturnsCoursesSuccessfully()
        {
            // Arrange
            var accountId = 1;
            var type = TrainingType.AllKeysTest;
            var courseDaos = new List<CourseDao>();
            var expectedCourseDtos = new List<CourseDto>();

            _courseRepositoryMock.Setup(x => x.GetCoursesByTypeAsync(accountId, type))
                .ReturnsAsync(courseDaos);
            _mapperMock.Setup(x => x.Map<IEnumerable<CourseDto>>(courseDaos))
                .Returns(expectedCourseDtos);

            // Act
            var returnedCourses = await _courseService.GetCoursesByType(accountId, type);
            var courseDtos = returnedCourses.ToList();

            // Assert
            courseDtos.Should().NotBeNull();
            courseDtos.Should().ContainSingle();
            _courseRepositoryMock.Verify(x => x.GetCoursesByTypeAsync(accountId, type), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<CourseDto>>(courseDaos), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_WithValidData_ReturnsCreatedCourse()
        {
            // Arrange
            var courseDto = new CourseDto
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                Name = "New Course",
                Type = TrainingType.Course,
                Settings = new CourseSetting(),
                LessonDataUrl = "test-url"
            };
            var courseDao = new CourseDao { Id = courseDto.Id, Name = courseDto.Name };
            var accountDao = new AccountDao { Id = courseDto.AccountId };

            _accountRepositoryMock.Setup(x => x.GetAccountByIdAsync(courseDto.AccountId))
                .ReturnsAsync(accountDao);
            _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseDto.Id))
                .ReturnsAsync((CourseDao?)null);
            _mapperMock.Setup(x => x.Map<CourseDao>(courseDto))
                .Returns(courseDao);
            _courseRepositoryMock.Setup(x => x.CreateCourseAsync(courseDao))
                .ReturnsAsync(courseDao);
            _mapperMock.Setup(x => x.Map<CourseDto>(courseDao))
                .Returns(courseDto);

            // Act
            var result = await _courseService.CreateCourse(courseDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(courseDto);
            _accountRepositoryMock.Verify(x => x.GetAccountByIdAsync(courseDto.AccountId), Times.Once);
            _courseRepositoryMock.Verify(x => x.CreateCourseAsync(courseDao), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_WithNullData_ReturnsNull()
        {
            // Act
            var result = await _courseService.CreateCourse(null);

            // Assert
            result.Should().BeNull();
            _accountRepositoryMock.Verify(x => x.GetAccountByIdAsync(It.IsAny<int>()), Times.Never);
            _courseRepositoryMock.Verify(x => x.CreateCourseAsync(It.IsAny<CourseDao>()), Times.Never);
        }

        [Fact]
        public async Task GenerateBeginnerCourse_WithValidSettings_ReturnsNewCourse()
        {
            // Arrange
            var settings = new CourseSetting
            {
                Minutes = 120,
                NewKeysPerStep = 1,
                PracticeTextLength = 74,
                TargetStats = new StatsBase { Wpm = 50, Accuracy = 90 }
            };

            // Act
            var result = await _courseService.GenerateBeginnerCourse(settings);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(TypingMasterConstants.BeginnerCourseName);
            result.Type.Should().Be(TrainingType.Course);
            result.Settings.Should().BeEquivalentTo(settings);
            result.Lessons.Should().NotBeNull();
        }

        [Fact]
        public async Task GenerateStartStats_ReturnsInitializedStats()
        {
            // Act
            var result = await _courseService.GenerateStartStats();

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(CourseService.CourseId1);
            result.LessonId.Should().Be(1);
            result.Wpm.Should().Be(0);
            result.Accuracy.Should().Be(0);
            result.KeyEvents.Should().NotBeNull();
            result.TypedText.Should().BeEmpty();
            result.StartTime.Should().NotBe(default);
            result.FinishTime.Should().NotBe(default);
        }

        [Fact]
        public async Task GetPracticeLesson_WhenCourseExists_ReturnsLesson()
        {
            // Arrange
            var testDataDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
            var testFilePath = Path.Combine(testDataDir, "test-practice-lessons.json");

            var courseId = Guid.NewGuid();
            var lessonId = 1;
            var stats = new StatsBase { Wpm = 40, Accuracy = 85 };
            var courseDao = new CourseDao
            {
                Id = courseId,
                Name = TypingMasterConstants.AllKeysCourseName,
                Type = TrainingType.AllKeysTest.ToString(),
                LessonDataUrl = testFilePath
            };
            var courseDto = new CourseDto
            {
                Id = courseId,
                Name = TypingMasterConstants.AllKeysCourseName,
                Type = TrainingType.AllKeysTest,
                LessonDataUrl = testFilePath,
                Settings = new CourseSetting()
            };

            _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseId))
                .ReturnsAsync(courseDao);
            _mapperMock.Setup(x => x.Map<CourseDto>(courseDao))
                .Returns(courseDto);

            // Act
            var result = await _courseService.GetPracticeLesson(courseId, lessonId, stats);

            // Assert
            result.Should().NotBeNull();
            _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.AtLeastOnce);
            _mapperMock.Verify(x => x.Map<CourseDto>(courseDao), Times.AtLeastOnce);
        }
    }
}