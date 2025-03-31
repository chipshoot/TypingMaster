using System.Text.Json;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course
{
    public class CourseFactory(ILogger logger) : ServiceBase(logger)
    {
        private static readonly Dictionary<string, List<Lesson>> LessonCache = new();
        private static readonly object CacheLock = new();

        public ICourse? CreateCourseInstance(CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return null;
            }

            return (courseDto.Type, courseDto.Name) switch
            {
                (TrainingType.Course, "BeginnerCourse") => new BeginnerCourse(logger)
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

                (TrainingType.Course, "AdvancedLevelCourse") => new AdvancedLevelCourse(logger)
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

                // Add more course types as needed

                _ => null // Return null for unsupported combinations
            };
        }


        public ICourse? GetInitializedCourse(CourseDto courseDto)
        {
            var course = CreateCourseInstance(courseDto);

            if (course == null) return null;

            // Initialize course-specific data
            switch (course)
            {
                case BeginnerCourse beginnerCourse:
                    beginnerCourse.Lessons = GetCachedBeginnerCourseLessons(beginnerCourse, logger);
                    break;

                case AdvancedLevelCourse advancedCourse:
                    // Initialize advanced course lessons if needed
                    // advancedCourse.Lessons = GenerateAdvancedCourseLessons(advancedCourse, _logger);
                    break;
            }

            return course;
        }

        /// <summary>
        /// Gets the beginner course lessons, either from cache or by loading them from the file
        /// </summary>
        private List<Lesson> GetCachedBeginnerCourseLessons(BeginnerCourse course, ILogger logger)
        {
            // Use course's LessonDataUrl as the cache key
            var cacheKey = course.LessonDataUrl;

            // Check if the lessons are already in the cache
            lock (CacheLock)
            {
                if (LessonCache.TryGetValue(cacheKey, out var cachedLessons))
                {
                    logger.Information($"Retrieved lessons from cache for {cacheKey}");
                    return [.. cachedLessons]; // Return a copy to prevent modification of cached data
                }

                // Not in cache, load from file
                var lessons = LoadLessonsFromFile(course);

                // Add to cache
                if (lessons.Count > 0)
                {
                    LessonCache[cacheKey] = new List<Lesson>(lessons); // Store a copy in the cache
                    logger.Information($"Added lessons to cache for {cacheKey}");
                }

                return lessons;
            }
        }

        /// <summary>
        /// Loads lesson data from a file and converts it to a list of Lesson objects
        /// </summary>
        private List<Lesson> LoadLessonsFromFile(ICourse course)
        {

            try
            {
                // Resolve the path relative to the executing assembly's location
                var assemblyLocation = Path.GetDirectoryName(typeof(CourseService).Assembly.Location);
                var fullPath = Path.Combine(assemblyLocation!, course.LessonDataUrl);

                if (!File.Exists(fullPath))
                {
                    ProcessResult.AddError($"Lesson data file not found at: {fullPath}");
                    return [];
                }

                var jsonString = File.ReadAllText(fullPath);
                var lessonDataList = JsonSerializer.Deserialize<List<LessonData>>(jsonString);

                if (lessonDataList == null)
                {
                    ProcessResult.AddError("Failed to deserialize lesson data");
                    return [];
                }

                // Convert LessonData objects to Lesson objects
                var lessons = lessonDataList.Select(lessonData => new Lesson
                {
                    Id = lessonData.Id,
                    Target = lessonData.Target,
                    Description = lessonData.Description,
                    Instruction = lessonData.Instruction,
                    Point = lessonData.Point,
                    PracticeText = string.Empty // Will be generated dynamically by BeginnerCourse.GetPracticeLesson
                }).ToList();

                return lessons;
            }
            catch (Exception ex)
            {
                ProcessResult.AddException(ex);
                return [];
            }
        }

        /// <summary>
        /// Clears the lesson cache for testing or when files have been updated
        /// </summary>
        public static void ClearLessonCache()
        {
            lock (CacheLock)
            {
                LessonCache.Clear();
            }
        }
    }
}