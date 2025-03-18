namespace TypingMaster.DataAccess.Dao;

public class DrillStatsDao
{
    public int Id { get; set; }

    public int PracticeLogId { get; set; }

    public Guid CourseId { get; set; }

    public int LessonId { get; set; }

    public string? PracticeText { get; set; }

    public string TypedText { get; set; } = null!;

    public Queue<KeyEventDao> KeyEventsJson { get; set; }

    public int Wpm { get; set; }

    public double Accuracy { get; set; }

    public int TrainingType { get; set; }

    // time stamp when the lesson starts
    public DateTime? StartTime { get; set; }

    // time stamp when the lesson is finished
    public DateTime? FinishTime { get; set; }
}