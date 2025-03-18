namespace TypingMaster.DataAccess.Dao;

public class KeyEventDao
{
    public char Key { get; set; }

    public char TypedKey { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime KeyDownTime { get; set; }

    public DateTime KeyUpTime { get; set; }

    public float Latency { get; set; }
}