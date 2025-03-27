namespace TypingMaster.Core.Models;

public class KeyStats : StatsBase
{
    public string Key { get; set; } = null!;

    public int TypingCount { get; set; }

    public int CorrectCount { get; set; }

    public double PressDuration { get; set; }

    public double Latency { get; set; }
}