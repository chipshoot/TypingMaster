using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services;

public interface ICourseWebService
{
    Task<CourseDto?> GetCourse(Guid id);

    Task<CourseDto?> GetAllKeysCourse(Guid? id);

    Task<CourseDto?> CreateCourse(CourseDto courseDto);

    Task<CourseDto?> GenerateBeginnerCourse(CourseSetting settings);

    Task<DrillStats?> GenerateStartStats();

    Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, StatsBase stats);
}