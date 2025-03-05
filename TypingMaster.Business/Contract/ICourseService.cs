namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        Task<ICourse?> GetCourse(int id);

        Task<ICourse?> GetAllKeysCourse(int? id);
    }
}