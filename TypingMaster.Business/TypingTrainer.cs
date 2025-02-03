using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class TypingTrainer(Account account) : ITypingTrainer
{
    private readonly Account _account = account ?? throw new ArgumentException(nameof(account));

    public SkillLevel GetSkillLevel(TypingStats stats)
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

    public string GetPracticeText(SkillLevel skillLevel)
    {
        var course = _account.CurrentCourse;
        return course.Materials.FirstOrDefault(x => x.Level == skillLevel)?.PracticeText ?? string.Empty;
    }

    public string GetPracticeText()
    {
        var currentLevel = GetSkillLevel(account.CurrentStats);
        return GetPracticeText(currentLevel);
    }
}