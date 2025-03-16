namespace TypingMaster.Server.Dao;

public class KeyStatsDao
{
    public string Key { get; set; } = null!;

    public int TypingCount { get; set; }

    public int CorrectCount { get; set; }

    public double PressDuration { get; set; }

    public double Latency { get; set; }

    public int Wpm { get; set; }

    public double Accuracy { get; set; }
}