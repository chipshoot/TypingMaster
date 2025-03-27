using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface ITypingTrainer
    {
        Account? Account { get; set; }

        TrainingType TrainingType { get; set; }

        void CheckPracticeResult(DrillStats stats);

        void ConvertKeyEventToKeyStats(Queue<KeyEvent> keyEvents);

        ProcessResult ProcessResult { get; set; }
    }
}