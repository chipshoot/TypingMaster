namespace TypingMaster.Core.Models;

public class StatsBase
{
    public int Wpm { get; set; }
    public double Accuracy { get; set; }

    public static bool operator >=(StatsBase stats1, StatsBase stats2)
    {
        return stats1.Wpm >= stats2.Wpm && stats1.Accuracy >= stats2.Accuracy;
    }

    public static bool operator <=(StatsBase stats1, StatsBase stats2)
    {
        return stats1.Wpm <= stats2.Wpm && stats1.Accuracy <= stats2.Accuracy;
    }
}