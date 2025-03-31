using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        Task<CourseDto?> GetCourse(Guid id);

        Task<CourseDto?> GetAllKeysCourse(Guid id);

        Task<CourseDto?> CreateCourse(CourseDto courseDto);

        Task<CourseDto> GenerateBeginnerCourse(CourseSetting settings);

        Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, StatsBase stats);

        Task<DrillStats> GenerateStartStats();
    }
}