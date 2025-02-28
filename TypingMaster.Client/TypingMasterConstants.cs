using TypingMaster.Business.Models;

namespace TypingMaster.Client;

public static class TypingMasterConstants
{
    public const string DefaultPracticePrompt = "Type the line in the top box. The arrow moves to show the next key to type. At the end of the line press either space or Enter.";

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
}