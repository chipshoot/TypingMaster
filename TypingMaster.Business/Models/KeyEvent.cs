namespace TypingMaster.Business.Models;

public class KeyEvent
{
    public char Key { get; set; }

    public char TypedKey { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime KeyDownTime { get; set; }

    public DateTime KeyUpTime { get; set; }

    public float Latency { get; set; }
}