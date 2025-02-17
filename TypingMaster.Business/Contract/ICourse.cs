using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface ICourse
{
    int Id { get; set; }

    IEnumerable<Lesson> Lessons { get; set; }

    string CompleteText { get; }

    Lesson? GetPracticeLesson(DrillStats stats);

    bool IsCompleted(DrillStats stats);

    DrillStats GenerateStartStats();

    string Description { get; }
}