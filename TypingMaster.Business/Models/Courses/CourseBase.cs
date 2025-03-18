using TypingMaster.Business.Contract;

namespace TypingMaster.Business.Models.Courses;

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

    public DrillStats GenerateStartStats()
    {
        return new DrillStats
        {
            CourseId = CourseService.CourseId1,
            LessonId = 1,
            Wpm = 0,
            Accuracy = 0,
            KeyEvents = [],
            TypedText = string.Empty,
            StartTime = DateTime.Now,
            FinishTime = DateTime.UtcNow
        };
    }
}