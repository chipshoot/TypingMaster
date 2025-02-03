using TypingMaster.Business.Models;

namespace TypingMaster.Business
{
    public interface ITypingTrainer
    {
        SkillLevel GetSkillLevel(TypingStats stats);

        string GetPracticeText(SkillLevel skillLevel);
    }
}