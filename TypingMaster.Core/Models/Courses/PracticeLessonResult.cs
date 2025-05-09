namespace TypingMaster.Core.Models.Courses;

public class PracticeLessonResult
{
    public Lesson Lesson { get; set; } = null!;

    public PracticePhases Phase { get; set; }
}