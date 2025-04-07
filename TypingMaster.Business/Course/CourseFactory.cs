using System.Text.Json;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
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
                (TrainingType.Course, TypingMasterConstants.BeginnerCourseName) => new BeginnerCourse(logger,
                    courseDto.LessonDataUrl)
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

                (TrainingType.Course, TypingMasterConstants.AdvancedLevelCourseName) => new AdvancedLevelCourse(logger,
                    courseDto.LessonDataUrl)
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

                (TrainingType.AllKeysTest, TypingMasterConstants.AllKeysCourseName) => new PracticeCourse(
                    TypingMasterConstants.AllKeysCourseName, logger, courseDto.LessonDataUrl, "")
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

                (TrainingType.SpeedTest, TypingMasterConstants.SpeedTestCourseName) => new PracticeCourse(
                    TypingMasterConstants.SpeedTestCourseName, logger, courseDto.LessonDataUrl, "")
                {
                    Id = courseDto.Id,
                    Name = courseDto.Name,
                    Settings = courseDto.Settings
                },

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
                    beginnerCourse.Lessons = GetCachedCourseLessons(beginnerCourse);
                    break;

                case AdvancedLevelCourse advancedCourse:
                    advancedCourse.Lessons = GetCachedCourseLessons(advancedCourse);
                    break;
                case PracticeCourse practiceCourse:
                    practiceCourse.Lessons = GetCachedCourseLessons(practiceCourse);
                    break;
            }

            return course;
        }

        /// <summary>
        /// Gets the beginner course lessons, either from cache or by loading them from the file
        /// </summary>
        private List<Lesson> GetCachedCourseLessons(ICourse course)
        {
            // Use course's LessonDataUrl as the cache key
            var cacheKey = course.LessonDataUrl;

            // Check if the lessons are already in the cache
            lock (CacheLock)
            {
                if (LessonCache.TryGetValue(cacheKey, out var cachedLessons))
                {
                    ProcessResult.AddInformation($"Retrieved lessons from cache for {cacheKey}");
                    return [.. cachedLessons]; // Return a copy to prevent modification of cached data
                }

                // Not in cache, load from file
                var lessons = LoadLessonsFromFile(course);

                // Add to cache
                if (lessons.Count > 0)
                {
                    LessonCache[cacheKey] = new List<Lesson>(lessons); // Store a copy in the cache
                    ProcessResult.AddInformation($"Added lessons to cache for {cacheKey}");
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
                    PracticeText = lessonData.PracticeText,
                    Description = lessonData.Description,
                    Instruction = lessonData.Instruction,
                    Point = lessonData.Point,
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