namespace TypingMaster.Business.Models;

public class DrillStats : StatsBase
{
    public Guid CourseId { get; set; }

    public int LessonId { get; set; }

    public string? PracticeText { get; set; }

    public string TypedText { get; set; } = null!;

    // hold the stats for each key in the lesson
    public Queue<KeyEvent> KeyEvents { get; set; } = null!;

    // time stamp when the lesson starts
    public DateTime? StartTime { get; set; }

    // time stamp when the lesson is finished
    public DateTime? FinishTime { get; set; }
}