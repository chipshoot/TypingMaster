using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface ITypingTrainer
    {
        string GetPracticeText(int lessonId);

        void CheckPracticeResult(DrillStats stats);
    }
}