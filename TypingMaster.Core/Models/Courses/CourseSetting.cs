
namespace TypingMaster.Core.Models.Courses;

public class CourseSetting
{
    public int Minutes { get; set; }

    public StatsBase TargetStats { get; set; } = null!;

    public int NewKeysPerStep { get; set; }

    /// <summary>
    /// The time of tries before trainer move to next phase.
    /// </summary>
    public int PhaseAttemptThreshold { get; set; }
}