
namespace TypingMaster.Core.Models.Courses;

public class CourseSetting
{
    public int Minutes { get; set; }

    public StatsBase TargetStats { get; set; } = null!;

    public int NewKeysPerStep { get; set; }

    public int PracticeTextLength { get; set; }
}