namespace TypingMaster.Business.Models;

public class StatsBase
{
    public int Wpm { get; set; }
    public double Accuracy { get; set; }

    public static bool operator >=(StatsBase left, StatsBase right)
    {
        if (left is null || right is null)
        {
            throw new ArgumentNullException(nameof(left), "StatsBase objects cannot be null");
        }

        return left.Wpm >= right.Wpm && left.Accuracy >= right.Accuracy;
    }

    public static bool operator <=(StatsBase left, StatsBase right)
    {
        if (left is null || right is null)
        {
            throw new ArgumentNullException(nameof(left), "StatsBase objects cannot be null");
        }

        return left.Wpm <= right.Wpm && left.Accuracy <= right.Accuracy;
    }
}