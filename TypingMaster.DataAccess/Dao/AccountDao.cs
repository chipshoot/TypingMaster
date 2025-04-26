using System.ComponentModel.DataAnnotations;

namespace TypingMaster.DataAccess.Dao;

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

    public Dictionary<string, object> Settings { get; set; } = [];

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency control
    /// </summary>
    [ConcurrencyCheck]
    public int Version { get; set; }
}