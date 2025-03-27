using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services;

public interface ICourseWebService
{
    Task<CourseBase?> GetCourse(Guid id);

    Task<CourseBase?> GetAllKeysCourse(Guid? id);

    Task<CourseBase> GenerateBeginnerCourse(CourseSetting settings);

    Task<DrillStats?> GenerateStartStats();

    Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, CourseSetting settings);
}