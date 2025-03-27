using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public interface ICourseRepository
{
    Task<CourseDao?> GetCourseByIdAsync(Guid id);

    Task<IEnumerable<CourseDao>> GetAllCoursesAsync();

    Task<CourseDao> CreateCourseAsync(CourseDao course);

    Task<CourseDao> UpdateCourseAsync(CourseDao course);

    Task<bool> DeleteCourseAsync(Guid id);

    Task<IEnumerable<CourseDao>> GetCoursesByTypeAsync(TrainingType type);

    Task<CourseDao?> GetCourseByNameAsync(string name);
}