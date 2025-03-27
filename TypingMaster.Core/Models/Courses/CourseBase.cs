using TypingMaster.Core.Contract;

namespace TypingMaster.Core.Models.Courses;

public abstract class CourseBase : ICourse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TrainingType Type { get; protected init; }

    public string LessonDataUrl { get; protected init; } = null!;

    public IEnumerable<Lesson> Lessons { get; set; }

    public string CompleteText { get; protected init; }

    public CourseSetting Settings { get; set; }

    public string Description { get; init; }

    public abstract Lesson? GetPracticeLesson(int curLessonId, StatsBase stats);

    public abstract bool IsCompleted(int curLessonId, StatsBase stats);
}