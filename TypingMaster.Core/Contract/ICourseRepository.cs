using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Core.Contract;

public interface ICourseRepository
{
    Task<CourseBase?> GetCourseByIdAsync(Guid id);
    Task<IEnumerable<CourseBase>> GetAllCoursesAsync();
    Task<CourseBase> CreateCourseAsync(CourseBase course);
    Task<CourseBase> UpdateCourseAsync(CourseBase course);
    Task<bool> DeleteCourseAsync(Guid id);
    Task<IEnumerable<CourseBase>> GetCoursesByTypeAsync(TrainingType type);
    Task<CourseBase?> GetCourseByNameAsync(string name);
}