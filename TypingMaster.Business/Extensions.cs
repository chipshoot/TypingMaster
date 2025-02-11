using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public static class Extensions
{
    public static SkillLevel GetSkillLevel(this TypingStats stats)
    {
        double pointWpm = stats.Wpm switch
        {
            < 30 => 1,
            < 45 => 2,
            < 60 => 3,
            < 75 => 4,
            _ => 5
        };

        double pointAccuracy = stats.Accuracy switch
        {
            < 80 => 1,
            < 85 => 2,
            < 90 => 3,
            < 95 => 4,
            _ => 5
        };

        var point = (pointWpm * 0.4) + (pointAccuracy * 0.6);
        return point switch
        {
            >= 1 and <= 1.9 => SkillLevel.Beginner,
            >= 2 and <= 2.9 => SkillLevel.Novice,
            >= 3 and <= 3.9 => SkillLevel.Intermediate,
            >= 4 and <= 4.9 => SkillLevel.Advanced,
            >= 5 => SkillLevel.Expert,
            _ => throw new ArgumentOutOfRangeException(nameof(stats))
        };
    }
}