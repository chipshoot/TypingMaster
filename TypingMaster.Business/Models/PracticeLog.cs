namespace TypingMaster.Business.Models;

public class PracticeLog
{
    public Guid CurrentCourseId { get; set; }

    public int CurrentLessonId { get; set; }

    public IEnumerable<DrillStats> PracticeStats { get; set; } = new List<DrillStats>();

    // Hold the states of each key of keyboard
    public Dictionary<char, KeyStats> KeyStats { get; set; } = new();

    // Total duration of the practice in hours
    public long PracticeDuration { get; set; }

}