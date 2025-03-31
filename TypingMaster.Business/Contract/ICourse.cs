using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract;

public interface ICourse
{
    Guid Id { get; set; }

    string Name { get; set; }

    TrainingType Type { get; }

    string LessonDataUrl { get; }

    IEnumerable<Lesson> Lessons { get; set; }

    string CompleteText { get; }

    CourseSetting Settings { get; set; }

    string Description { get; }

    Lesson? GetPracticeLesson(int curLessonId, StatsBase stats);
}