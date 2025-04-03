using AutoMapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Course;
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

    // todo: create a new way to get speed test course
    // todo: move the text to a JSON data file
    //course = new AdvancedLevelCourse()
    //{
    //    Id = AllKeyTestCourseId,
    //    Lessons =
    //        [
    //            new Lesson
    //            {
    //                Id = 1,
    //                PracticeText = "hAnother sharp-witted character, _;  The aerialist's phone number was (217) 389-7557.  The freeloaders expanded by this den located at 97191 Straus Square, Piscataway, Ohio 67107-0614! \"Boy!\" she vocalized.  This understandably zealous physician was jesting regarding my executive around 23 Heyl Plaza, Quantico, Alabama 92780-8450!  The jaywalkers hurled beside the piazza located at 57 Arrington Center, Hatfield, DE",
    //                Point = 1,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            },
    //            new Lesson
    //            {
    //                Id = 2,
    //                PracticeText = "Taking a test will give this program a basic for customizing individual lessons. Don't be concerned if you aren't typing as quickly as you can; you will soon excel.  Everyone has a quandary with number and symbol keys.  Don't expect to zoom through these next sentences - just do your best! \"Her birthday is 08/154/53.\"  Does 73% of $546.00 = $98.58? Item #32 (jonquils)  sold 2 units @ $6 & 1 @ $73. *+* That's all folks! +*+",
    //                Point = 1,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            },
    //            new Lesson
    //            {
    //                Id = 3,
    //                PracticeText = "SHIFTING thoughts, typing FLUENTLY, developing SPEED! Caps Lock may be a friend or foe—KNOW when to use it. Function keys (F1-F12) aid shortcuts. Ctrl + C copies, Ctrl + V pastes, and Alt + Tab swaps. The Escape key, a way out. Don't ignore the power of the ENTER key—confirming, submitting, breaking lines.  Every key counts, so practice with purpose!",
    //                Point = 1,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            },
    //            new Lesson
    //            {
    //                Id = 4,
    //                PracticeText = "Coding requires symbols: {} for blocks, [] for arrays, <> for comparisons. The backslash \\ is essential in file paths, escaping characters. Quotes \"\" and '' frame strings, while ` allows inline code. The spacebar, a silent hero, controls rhythm. Typing, like music, has a flow—staccato or smooth.  To improve, practice! Type, correct, repeat, master.",
    //                Point = 2,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            },
    //            new Lesson
    //            {
    //                Id = 5,
    //                PracticeText = "Emails need @, URLs need .com, passwords mix Aa1! Common shortcuts: Ctrl + S saves, Ctrl + Z undoes, Ctrl + X cuts. Mac users prefer Cmd over Ctrl. The numpad—quicker digits, faster math. Ergonomic posture boosts accuracy. Fingers rest on ASDF JKL;—home row anchors speed. Keep going, improving, perfecting!  Type on, challenge yourself, enjoy the process.",
    //                Point = 3,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            },
    //            new Lesson
    //            {
    //                Id = 6,
    //                PracticeText = "Practice drills: Type \"The $ sign matters\"—spot it? Press Tab \u2192 it indents. Press Backspace \u2190 it erases. Use Delete \u27a1 it clears ahead. Adjust Caps \u21ea if needed. Special characters? Try Alt + 0153 (\u2122) or Alt + 0169 (\u00a9). Ever typed in different languages? ¡Sí! Voilà! 汉字也行. Typing unites, connects, empowers.  Never stop—keep practicing!",
    //                Point = 4,
    //                Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
    //            }
    //        ]
    //};
    //_courses.Add(course);

    public async Task<CourseDto?> GetCourse(Guid id)
    {
        try
        {
            var courseDao = await courseRepository.GetCourseByIdAsync(id);
            var course = mapper.Map<CourseDto>(courseDao);
            return course;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<CourseDto?> GetAllKeysCourse(Guid id)
    {
        try
        {
            var courseDaos = await (courseRepository.GetCoursesByTypeAsync(TrainingType.AllKeysTest));
            var courseDao = courseDaos.FirstOrDefault(c => c.Id == id);
            if (courseDao != null)
            {
                var course = mapper.Map<CourseDto>(courseDao);
                return course;
            }

            ProcessResult.AddError($"Course with ID {id} not found");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<CourseDto?> GetDiskTestCourse(Guid id)
    {
        try
        {
            var courseDaos = await (courseRepository.GetCoursesByTypeAsync(TrainingType.SpeedTest));
            var courseDao = courseDaos.FirstOrDefault(c => c.Id == id);
            if (courseDao != null)
            {
                var course = mapper.Map<CourseDto>(courseDao);
                return course;
            }

            ProcessResult.AddError($"Course with ID {id} not found");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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

    public Task<CourseDto> GenerateDiskTestCourse(CourseSetting settings)
    {
        var course = new CourseDto
        {
            Id = Guid.NewGuid(),
            Name = "DiskTestCourse",
            Settings = settings,
            Lessons = new List<Lesson>(),
            Type = TrainingType.Course,
            LessonDataUrl = string.Empty,
            Description = "This is a disk test (speed test) course for new typists"
        };

        return Task.FromResult(course);
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
            course.GetPracticeLesson(lessonId, stats);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
        return await ProcessCourse(courseId, stats, (course, targetStats) => course.GetPracticeLesson(lessonId, targetStats));
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
                (TrainingType.Course, "BeginnerCourse") => "Resources/LessonData/beginner-course-lessons.json",
                (TrainingType.Course, "AdvancedLevelCourse") => "Resources/LessonData/advanced-level-course-lessons.json",
                (TrainingType.AllKeysTest,"AllKeysTest" ) => "Resources/LessonData/all-keys-test-course-lessons.json",
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