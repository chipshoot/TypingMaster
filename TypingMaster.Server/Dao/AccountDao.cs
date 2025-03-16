namespace TypingMaster.Server.Dao;

public class AccountDao
{
    public int Id { get; set; }

    public string AccountName { get; set; } = null!;

    public string AccountEmail { get; set; } = null!;

    public StatsDao GoalStats { get; set; } = null!;

    // Navigation properties
    public UserProfileDao User { get; set; } = null!;

    public PracticeLogDao History { get; set; } = null!;

    public ICollection<CourseDao> Courses { get; set; } = new List<CourseDao>();
}