namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        Task<ICourse> GetCourse(int id);
    }
}