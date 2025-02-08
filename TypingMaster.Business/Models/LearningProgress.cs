namespace TypingMaster.Business.Models;

public class LearningProgress
{
    public int CourseId { get; set; }

    public int LessonId { get; set; }

    public TypingStats? Stats { get; set; } // stats for current lesson

    public DateTime? FinishDate { get; set; } // time stamp when the lesson is finished
}