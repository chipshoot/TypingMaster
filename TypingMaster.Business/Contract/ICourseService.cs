using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        Task<ICourse?> GetCourse(Guid id);

        Task<ICourse?> GetAllKeysCourse(Guid? id);

        Task<ICourse> GenerateBeginnerCourse(CourseSetting settings);
    }
}