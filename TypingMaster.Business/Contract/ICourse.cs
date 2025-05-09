using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Contract;

public interface ICourse
{
    Guid Id { get; set; }

    string Name { get; set; }

    TrainingType Type { get; }

    string LessonDataUrl { get; }

    IEnumerable<Lesson> Lessons { get; set; }

    string CompleteText { get; }

    int MaxCharacters { get; set; }

    CourseSetting Settings { get; set; }

    string Description { get; }

    /// <summary>
    /// Advances the trainer to the next phase based on the current phase.
    /// </summary>
    /// <param name="phase">The current practice phase</param>
    /// <param name="currentStats">The user's current typing statistics</param>
    /// <param name="lessonType">The type of current lesson</param>
    /// <returns>The next practice phase</returns>
    public PracticePhases AdvanceToNextPhase(PracticePhases phase, LessonType lessonType, StatsBase currentStats);

    PracticeLessonResult? GetPracticeLesson(int curLessonId, StatsBase stats, PracticePhases phase);

    ProcessResult ProcessResult { get; set; }
}