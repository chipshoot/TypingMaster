namespace TypingMaster.Business.Models
{
    public class Course
    {
        public int Id { get; set; }
        public List<Lesson> Lessons { get; set; } = [];
    }
}