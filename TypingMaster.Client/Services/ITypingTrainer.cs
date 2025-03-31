using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services
{
    public interface ITypingTrainer
    {
        Account? Account { get; }

        void SetupTrainer(Account account, CourseDto course);

        void CheckPracticeResult(DrillStats stats);

        void ConvertKeyEventToKeyStats(Queue<KeyEvent> keyEvents);

        ProcessResult ProcessResult { get; set; }
    }
}