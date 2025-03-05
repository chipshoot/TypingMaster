using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface ICourse
{
    int Id { get; set; }

    string Name { get; set; }

    CourseType Type { get; }

    IEnumerable<Lesson> Lessons { get; set; }

    string CompleteText { get; }

    Lesson? GetPracticeLesson(DrillStats stats);

    bool IsCompleted(DrillStats stats);

    DrillStats GenerateStartStats();

    string Description { get; }
}