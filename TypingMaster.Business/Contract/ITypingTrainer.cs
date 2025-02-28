using TypingMaster.Business.Models;
using TypingMaster.Business.Utility;

namespace TypingMaster.Business.Contract
{
    public interface ITypingTrainer
    {
        Account? Account { get; set; }

        void CheckPracticeResult(DrillStats stats);

        void ConvertKeyEventToKeyStats(Queue<KeyEvent> keyEvents);

        ProcessResult ProcessResult { get; set; }
    }
}