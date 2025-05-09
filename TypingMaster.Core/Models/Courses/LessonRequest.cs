namespace TypingMaster.Core.Models.Courses;

public class LessonRequest
{
    public Guid CourseId { get; set; }

    public int LessonId { get; set; }

    public StatsBase Stats { get; set; } = null!;

    public PracticePhases Phase { get; set; }

    public int MaxCharacters { get; set; }
}