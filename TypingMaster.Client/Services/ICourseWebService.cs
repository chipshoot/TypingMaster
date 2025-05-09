using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services;

public interface ICourseWebService
{
    Task<CourseDto?> GetCourse(Guid id);

    Task<IEnumerable<CourseDto>> GetCoursesByType(int accountId, TrainingType type);

    Task<CourseDto?> GetCoursesByTypeForGuest(TrainingType type);

    Task<CourseDto?> CreateCourse(CourseDto courseDto);

    Task<CourseDto?> GenerateBeginnerCourse(CourseSetting settings);

    Task<DrillStats?> GenerateStartStats();

    Task<PracticeLessonResult?> GetPracticeLesson(Guid courseId, int lessonId, StatsBase stats, PracticePhases phase, int maxCharacters);
}