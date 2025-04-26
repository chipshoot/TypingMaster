namespace TypingMaster.Core.Models;

public static class Extensions
{
    public static SkillLevel GetSkillLevel(this StatsBase stats)
    {
        return (stats.Wpm, stats.Accuracy).GetSkillLevel();
    }

    public static SkillLevel GetSkillLevel(this (int Wpm, double Accuracy) stats)
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

    public static Queue<KeyEvent> Copy(this Queue<KeyEvent> originalQueue)
    {
        return new Queue<KeyEvent>(originalQueue.Select(k => new KeyEvent
        {
            Key = k.Key,
            TypedKey = k.TypedKey,
            IsCorrect = k.IsCorrect,
            KeyDownTime = k.KeyDownTime,
            KeyUpTime = k.KeyUpTime,
            Latency = k.Latency
        }));
    }

    public static int ConvertToInt(this object? value, int defaultValue)
    {
        if (value is int intValue)
            return intValue;
    
        if (value != null && int.TryParse(value.ToString(), out var parsedValue))
            return parsedValue;
    
        return defaultValue;
    }
}