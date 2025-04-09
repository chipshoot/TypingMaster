using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Tests.Course;

public class CourseRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ICourseRepository _repository;

    public CourseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Setup test data
        SeedTestData();

        // Create logger mock
        var mockLogger = new Mock<ILogger>();
        _repository = new CourseRepository(_context, mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GenerateTestCourseDao_ReturnsValidCourseDao()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Generated Test Course",
            Description = "Generated Test Description",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PracticeTextLength = 50,
                TargetStats = new StatsDao
                {
                    Wpm = 25,
                    Accuracy = 85
                }
            }
        };

        // Act
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        var savedCourse = await _context.Courses.FindAsync(course.Id);

        // Assert
        Assert.NotNull(savedCourse);
        Assert.Equal(course.Id, savedCourse.Id);
        Assert.Equal(course.Name, savedCourse.Name);
        Assert.Equal(course.Description, savedCourse.Description);
        Assert.Equal(course.Type, savedCourse.Type);
        Assert.Equal(course.SettingsJson.Minutes, savedCourse.SettingsJson.Minutes);
        Assert.Equal(course.SettingsJson.NewKeysPerStep, savedCourse.SettingsJson.NewKeysPerStep);
        Assert.Equal(course.SettingsJson.PracticeTextLength, savedCourse.SettingsJson.PracticeTextLength);
        Assert.Equal(course.SettingsJson.TargetStats.Wpm, savedCourse.SettingsJson.TargetStats.Wpm);
        Assert.Equal(course.SettingsJson.TargetStats.Accuracy, savedCourse.SettingsJson.TargetStats.Accuracy);
    }

    [Fact]
    public async Task GetCourseDaoByIdAsync_WhenCourseExists_ReturnsCourse()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Test Course",
            Description = "Description for Test Course",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PracticeTextLength = 50,
                TargetStats = new StatsDao
                {
                    Wpm = 25,
                    Accuracy = 85
                }
            }
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseByIdAsync(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        Assert.Equal(course.Name, result.Name);
        Assert.Equal(course.Description, result.Description);
        Assert.Equal(course.Type, result.Type);
    }

    [Fact]
    public async Task GetCourseDaoByIdAsync_WhenCourseDoesNotExist_ReturnsNull()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var result = await _repository.GetCourseByIdAsync(courseId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllCoursesAsync_ReturnsAllCourses()
    {
        // Arrange
        // Act
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllCoursesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateCourseDaoAsync_AddsCourseToDatabase()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Test Created Course",
            Description = "A course created for testing the repository",
            Type = ((int)TrainingType.Course).ToString(),
            LessonDataUrl = "Resources/LessonData/test-course-lessons.json",
            SettingsJson = new CourseSettingDao
            {
                Minutes = 45,
                NewKeysPerStep = 2,
                PracticeTextLength = 75,
                TargetStats = new StatsDao
                {
                    Wpm = 35,
                    Accuracy = 92
                }
            }
        };

        // Act
        var result = await _repository.CreateCourseAsync(course);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        var savedCourse = await _context.Courses.FindAsync(course.Id);
        Assert.NotNull(savedCourse);
        Assert.Equal(course.Name, savedCourse.Name);
    }

    [Fact]
    public async Task SampleCourseDao_ReturnsValidCourseDao()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Sample Course",
            Description = "Sample Description",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 30,
                NewKeysPerStep = 1,
                PracticeTextLength = 100,
                TargetStats = new StatsDao
                {
                    Wpm = 20,
                    Accuracy = 80
                }
            }
        };

        // Act
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        var savedCourse = await _context.Courses.FindAsync(course.Id);

        // Assert
        Assert.NotNull(savedCourse);
        Assert.Equal(course.Id, savedCourse.Id);
        Assert.Equal(course.Name, savedCourse.Name);
        Assert.Equal(course.Description, savedCourse.Description);
        Assert.Equal(course.Type, savedCourse.Type);
        Assert.Equal(course.SettingsJson.Minutes, savedCourse.SettingsJson.Minutes);
        Assert.Equal(course.SettingsJson.NewKeysPerStep, savedCourse.SettingsJson.NewKeysPerStep);
        Assert.Equal(course.SettingsJson.PracticeTextLength, savedCourse.SettingsJson.PracticeTextLength);
        Assert.Equal(course.SettingsJson.TargetStats.Wpm, savedCourse.SettingsJson.TargetStats.Wpm);
        Assert.Equal(course.SettingsJson.TargetStats.Accuracy, savedCourse.SettingsJson.TargetStats.Accuracy);
    }

    [Fact]
    public async Task DeleteCourseDaoAsync_WhenCourseExists_DeletesCourse()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Sample Course",
            Description = "Sample Description",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 30,
                NewKeysPerStep = 1,
                PracticeTextLength = 100,
                TargetStats = new StatsDao
                {
                    Wpm = 20,
                    Accuracy = 80
                }
            }
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteCourseAsync(course.Id);

        // Assert
        Assert.True(result);
        var deletedCourse = await _context.Courses.FindAsync(course.Id);
        Assert.Null(deletedCourse);
    }

    [Fact]
    public async Task DeleteCourseDaoAsync_WhenCourseDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteCourseAsync(courseId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetCoursesDaoByTypeAsync_ReturnsCoursesOfSpecifiedType()
    {
        // Arrange
        var courses = new List<CourseDao>
       {
           new()
           {
               Id = Guid.NewGuid(),
               AccountId = 1,
               Name = "Sample Course 1",
               Description = "Description for Sample Course 1",
               Type = ((int)TrainingType.Game).ToString(),
               SettingsJson = new CourseSettingDao
               {
                   Minutes = 60,
                   NewKeysPerStep = 1,
                   PracticeTextLength = 50,
                   TargetStats = new StatsDao
                   {
                       Wpm = 25,
                       Accuracy = 85
                   }
               }
           },
           new()
           {
               Id = Guid.NewGuid(),
               AccountId = 1,
               Name = "Sample Course 2",
               Description = "Description for Sample Course 2",
               Type = ((int)TrainingType.Game).ToString(),
               SettingsJson = new CourseSettingDao
               {
                   Minutes = 90,
                   NewKeysPerStep = 2,
                   PracticeTextLength = 100,
                   TargetStats = new StatsDao
                   {
                       Wpm = 30,
                       Accuracy = 90
                   }
               }
           }
       };

        _context.Courses.AddRange(courses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCoursesByTypeAsync(1, TrainingType.Game);

        // Assert
        Assert.NotNull(result);
        var courseDaos = result as CourseDao[] ?? result.ToArray();
        Assert.Equal(2, courseDaos.Count());
        Assert.All(courseDaos, c => Assert.Equal(((int)TrainingType.Game).ToString(), c.Type));
    }

    [Fact]
    public async Task GetCourseDaoByNameAsync_WhenCourseExists_ReturnsCourse()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Test Course",
            Description = "Description for Test Course",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PracticeTextLength = 50,
                TargetStats = new StatsDao
                {
                    Wpm = 25,
                    Accuracy = 85
                }
            }
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseByNameAsync("Test Course");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Course", result.Name);
    }

    [Fact]
    public async Task GetCourseDaoByNameAsync_WhenCourseDoesNotExist_ReturnsNull()
    {
        // Arrange
        var courseName = "Non-existent Course";

        // Act
        var result = await _repository.GetCourseByNameAsync(courseName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCourseDaoAsync_WhenCourseExists_UpdatesCourse()
    {
        // Arrange
        var course = new CourseDao
        {
            Id = Guid.NewGuid(),
            AccountId = 1,
            Name = "Original Course Name",
            Description = "Original Description",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = new CourseSettingDao
            {
                Minutes = 60,
                NewKeysPerStep = 1,
                PracticeTextLength = 50,
                TargetStats = new StatsDao
                {
                    Wpm = 25,
                    Accuracy = 85
                }
            }
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Update the course
        course.Name = "Updated Course Name";
        course.Description = "Updated Description";
        course.SettingsJson.Minutes = 90;

        // Act
        var result = await _repository.UpdateCourseAsync(course);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        Assert.Equal("Updated Course Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(90, result.SettingsJson.Minutes);

        // Verify the changes are persisted in the database
        var updatedCourse = await _context.Courses.FindAsync(course.Id);
        Assert.NotNull(updatedCourse);
        Assert.Equal("Updated Course Name", updatedCourse.Name);
        Assert.Equal("Updated Description", updatedCourse.Description);
        Assert.Equal(90, updatedCourse.SettingsJson.Minutes);
    }

    private void SeedTestData()
    {
        // Create CourseSettingDao objects for each course
        var settings1 = new CourseSettingDao
        {
            Minutes = 120,
            NewKeysPerStep = 2,
            PracticeTextLength = 100,
            TargetStats = new StatsDao
            {
                Wpm = 30,
                Accuracy = 90
            }
        };

        var settings2 = new CourseSettingDao
        {
            Minutes = 180,
            NewKeysPerStep = 3,
            PracticeTextLength = 150,
            TargetStats = new StatsDao
            {
                Wpm = 45,
                Accuracy = 95
            }
        };

        // Add practice logs first to get assigned IDs
        var course1 = new CourseDao
        {
            Id = Guid.Parse("AB7E8988-4E54-435F-9DC3-25D3193EC378"),
            AccountId = 1,
            Name = TypingMasterConstants.AllKeysCourseName,
            LessonDataUrl = "Resources/LessonData/beginner-course-lessons.json",
            Description = "A course for beginners to learn touch typing from scratch",
            Type = ((int)TrainingType.Course).ToString(),
            SettingsJson = settings1
        };

        var course2 = new CourseDao
        {
            Id = Guid.Parse("B326B0D9-F44C-4206-BE3B-301824817EEA"),
            AccountId = 1,
            Name = TypingMasterConstants.AllKeysCourseName,
            LessonDataUrl = "Resources/LessonData/all-keys-test-lessons.json",
            Description = "A comprehensive test for all keyboard keys and typing proficiency",
            Type = ((int)TrainingType.AllKeysTest).ToString(),
            SettingsJson = settings2
        };

        _context.Courses.AddRange(course1, course2);
        _context.SaveChanges();
    }
}