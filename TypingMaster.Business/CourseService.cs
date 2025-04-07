using AutoMapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Course;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class CourseService(ICourseRepository courseRepository, IAccountRepository accountRepository, IMapper mapper, ILogger logger, IConfiguration configuration)
    : ServiceBase(logger), ICourseService
{
    private readonly CourseFactory _courseFactory = new CourseFactory(logger);

    public static Guid CourseId1 = new("AB7E8988-4E54-435F-9DC3-25D3193EC378");

    public async Task<CourseDto?> GetCourse(Guid id)
    {
        try
        {
            var courseDao = await courseRepository.GetCourseByIdAsync(id);
            if (courseDao == null)
            {
                return null;
            }

            var course = mapper.Map<CourseDto>(courseDao);
            return course;

        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesByType(int accountId, TrainingType type)
    {
        try
        {
            var courseDaos = await courseRepository.GetCoursesByTypeAsync(accountId, type);
            var courses = mapper.Map<IEnumerable<CourseDto>>(courseDaos);
            var coursesByTypes = courses.ToList();

            // If no courses found for user, add a default course
            if (!coursesByTypes.AsEnumerable().Any())
            {
                var setting = new CourseSetting
                {
                    Minutes = 120,
                    NewKeysPerStep = 1,
                    PracticeTextLength = 74,
                    TargetStats = new StatsBase { Wpm = 50, Accuracy = 90 }
                };
                CourseDto? course = null;
                switch (type)
                {
                    case TrainingType.AllKeysTest:
                    case TrainingType.SpeedTest:
                        course = await GeneratePracticeCourse(accountId, type, setting);
                        break;
                }

                if (course != null)
                {
                    coursesByTypes.Add(course);
                }
            }
            else
            {
                // If courses are found, we need add lesson for practice course
                if (type is TrainingType.AllKeysTest or TrainingType.SpeedTest)
                {
                    foreach (var item in coursesByTypes)
                    {
                        var practiceCourse = _courseFactory.GetInitializedCourse(item);
                        if (practiceCourse != null)
                        {
                            item.Lessons = practiceCourse.Lessons;
                        }
                    }
                }
            }

            return coursesByTypes;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return new List<CourseDto>();
        }
    }

    public async Task<CourseDto?> CreateCourse(CourseDto courseDto)
    {
        // Validate courseDto is not null
        if (courseDto == null)
        {
            ProcessResult.AddError("Course data cannot be null");
            return null;
        }

        try
        {
            // Validate that Account exists in system
            // Using dependency injection, we'd need an IAccountService, but it would cause circular dependency
            // So we'll need to use ICourseRepository to verify AccountId directly
            var account = await accountRepository.GetAccountByIdAsync(courseDto.AccountId);
            if (account == null)
            {
                ProcessResult.AddError($"Account with ID {courseDto.AccountId} not found");
                return null;
            }

            // Validate that course ID is unique, ensuring we don't overwrite existing courses
            if (courseDto.Id == Guid.Empty)
            {
                // Generate a new ID if not provided
                courseDto.Id = Guid.NewGuid();
            }
            else
            {
                // Check if the ID already exists
                var existingCourse = await courseRepository.GetCourseByIdAsync(courseDto.Id);
                if (existingCourse != null)
                {
                    ProcessResult.AddError($"A course with ID {courseDto.Id} already exists");
                    return null;
                }
            }

            // Validate that the course has a valid type
            if (!Enum.IsDefined(typeof(TrainingType), courseDto.Type))
            {
                ProcessResult.AddError($"Invalid training type: {courseDto.Type}");
                return null;
            }

            // Validate that settings is not null
            if (courseDto.Settings == null)
            {
                ProcessResult.AddError("Course settings cannot be null");
                return null;
            }

            // Additional validations for required fields
            if (string.IsNullOrWhiteSpace(courseDto.Name))
            {
                ProcessResult.AddError("Course name is required");
                return null;
            }

            if (string.IsNullOrWhiteSpace(courseDto.LessonDataUrl))
            {
                var defaultUrl = GetDefaultLessonDataUrl(courseDto.Type, courseDto.Name);
                if (string.IsNullOrWhiteSpace(defaultUrl))
                {
                    ProcessResult.AddError($"No default lesson data URL found for course type: {courseDto.Type} and name: {courseDto.Name}");
                    return null;
                }
                courseDto.LessonDataUrl = defaultUrl;
            }

            if (courseDto.Lessons == null || !courseDto.Lessons.Any())
            {
                // Initialize with empty lesson collection if none provided
                courseDto.Lessons = new List<Lesson>();
            }

            // Map the DTO to DAO and save it
            var courseDao = mapper.Map<CourseDao>(courseDto);
            var createdCourseDao = await courseRepository.CreateCourseAsync(courseDao);

            // Map back to DTO and return
            var createdCourseDto = mapper.Map<CourseDto>(createdCourseDao);

            ProcessResult.AddSuccess();
            return createdCourseDto;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public Task<CourseDto> GenerateBeginnerCourse(CourseSetting settings)
    {
        var course = new CourseDto
        {
            Id = Guid.NewGuid(),
            Name = "BeginnerCourse",
            Settings = settings,
            Lessons = new List<Lesson>(),
            Type = TrainingType.Course,
            LessonDataUrl = string.Empty,
            Description = "This is a beginner course for new typists"
        };

        return Task.FromResult(course);
    }

    public Task<CourseDto> GeneratePracticeCourse(int accountId, TrainingType type, CourseSetting settings)
    {
        var courseFileName = type switch
        {
            TrainingType.AllKeysTest => TypingMasterConstants.AllKeysCourseName,
            TrainingType.SpeedTest => TypingMasterConstants.SpeedTestCourseName,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        var course = new CourseDto
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Name = courseFileName,
            Settings = settings,
            Lessons = new List<Lesson>(),
            Type = type,
            LessonDataUrl = GetDefaultLessonDataUrl(type, courseFileName),
            Description = "This is a disk test (speed test) course for new typists"
        };

        var courseTemplate = _courseFactory.GetInitializedCourse(course);
        if (courseTemplate != null)
        {
            course.Lessons= courseTemplate.Lessons;
        }

        return Task.FromResult(course);
    }

    public Task<DrillStats> GenerateStartStats()
    {
        var stats = new DrillStats
        {
            CourseId = CourseService.CourseId1,
            LessonId = 1,
            Wpm = 0,
            Accuracy = 0,
            KeyEvents = new Queue<KeyEvent>(),
            TypedText = string.Empty,
            StartTime = DateTime.Now,
            FinishTime = DateTime.UtcNow
        };

        return Task.FromResult(stats);
    }

    public async Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, StatsBase stats)
    {
        if (stats == null)
        {
            ProcessResult.AddError($"Current stats cannot found");
            return null;
        }

        try
        {
            // get course information
            var courseDao = await courseRepository.GetCourseByIdAsync(courseId);
            if (courseDao == null)
            {
                ProcessResult.AddError($"Course: {courseId} not found");
                return null;
            }

            var courseDto = mapper.Map<CourseDto>(courseDao);

            // Use the factory to create and initialize the appropriate course instance
            var course = _courseFactory.GetInitializedCourse(courseDto);
            if (course == null)
            {
                ProcessResult.AddError($"Unsupported course type: {courseDto.Type} with name: {courseDto.Name}");
                return null;
            }

            // Process the course with the provided function
            return course.GetPracticeLesson(lessonId, stats);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    // Helper method to get default lesson data URL based on course type and name
    private string GetDefaultLessonDataUrl(TrainingType type, string name)
    {
        var defaultUrlsSection = configuration.GetSection("CourseSettings:DefaultLessonUrls");

        // Try to get the URL based on the course name first
        var url = defaultUrlsSection[name];

        // If not found, use a fallback approach based on type
        if (string.IsNullOrWhiteSpace(url))
        {
            url = (type, name) switch
            {
                (TrainingType.Course, TypingMasterConstants.BeginnerCourseName) => "Resources/LessonData/beginner-course-lessons.json",
                (TrainingType.Course, TypingMasterConstants.AdvancedLevelCourseName) => "Resources/LessonData/advanced-level-course-lessons.json",
                (TrainingType.AllKeysTest, TypingMasterConstants.AllKeysCourseName) => "Resources/LessonData/all-keys-test-course-lessons.json",
                (TrainingType.SpeedTest, TypingMasterConstants.SpeedTestCourseName) => "Resources/LessonData/disk-test-course-lessons.json",
                _ => string.Empty
            };
        }

        return url;
    }

    private async Task<T?> ProcessCourse<T>(Guid courseId, StatsBase stats, Func<ICourse, StatsBase, T?> processFunc)
    {
        try
        {
            // get course information
            var courseDao = await courseRepository.GetCourseByIdAsync(courseId);
            if (courseDao == null)
            {
                ProcessResult.AddError($"Course: {courseId} not found");
                return default;
            }

            var courseDto = mapper.Map<CourseDto>(courseDao);

            // Use the factory to create and initialize the appropriate course instance
            var course = _courseFactory.GetInitializedCourse(courseDto);
            if (course == null)
            {
                ProcessResult.AddError($"Unsupported course type: {courseDto.Type} with name: {courseDto.Name}");
                return default;
            }

            // Process the course with the provided function
            return processFunc(course, stats);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return default;
        }
    }
}