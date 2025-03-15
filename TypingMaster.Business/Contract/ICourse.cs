using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface ICourse
{
    Guid Id { get; set; }

    string Name { get; set; }

    TrainingType Type { get; }

    IEnumerable<Lesson> Lessons { get; set; }

    string CompleteText { get; }

    Lesson? GetPracticeLesson(int curLessonId, StatsBase stats);

    public CourseSetting Settings { get; set; }

    bool IsCompleted(int curLessonId, StatsBase stats);

    DrillStats GenerateStartStats();

    string Description { get; }
}