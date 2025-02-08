using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface ICourseService
    {
        public Course GetCourse(int id);
    }
}