using TypingMaster.Core.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        Task<ICourse?> GetCourse(Guid id);

        Task<ICourse?> GetAllKeysCourse(Guid? id);

        Task<ICourse> GenerateBeginnerCourse(CourseSetting settings);

        Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, CourseSetting settings);

        Task<DrillStats> GenerateStartStats();
    }
}