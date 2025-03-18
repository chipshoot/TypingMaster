namespace TypingMaster.DataAccess.Dao;

public class PracticeLogDao
{
    public int Id { get; set; }

    public Guid CurrentCourseId { get; set; }

    public int CurrentLessonId { get; set; }

    public ICollection<DrillStatsDao> PracticeStats { get; set; } = new List<DrillStatsDao>();

    public Dictionary<char, KeyStatsDao> KeyStatsJson { get; set; }

    public long PracticeDuration { get; set; }
}
