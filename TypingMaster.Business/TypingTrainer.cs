using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class TypingTrainer(Account account) : ITypingTrainer
{
    private readonly Account _account = account ?? throw new ArgumentException(nameof(account));
    private readonly Course _course = account.CurrentCourse ?? throw new ArgumentException(nameof(account.CurrentCourse));

    public string GetPracticeText(int lessonId)
    {
        return _course.Lessons.FirstOrDefault(x => x.Id == lessonId)?.PracticeText ?? string.Empty;
    }

    public (Lesson? lesson, string practiceText) GetPracticeText()
    {
        if (_account.Progress == null) throw new InvalidOperationException("Progress not found.");
        if (_course.Lessons == null) throw new InvalidOperationException("Course lesson not found.");

        LearningProgress currentProgress;

        if (_account.Progress.Any())
        {
            currentProgress = _account.Progress.Last();
        }
        else
        {
            currentProgress = new LearningProgress
            {
                CourseId = _account.CurrentCourse.Id,
                LessonId = 0,
                Stats = null
            };
        }
        var curLessonId = currentProgress.LessonId;
        var curLessonPoint = _course.Lessons.FirstOrDefault(x => x.Id == curLessonId)?.Point ?? 1;
        var curSkill = currentProgress.Stats?.GetSkillLevel() ?? SkillLevel.Beginner;

        // if current skill level above advanced, return the lesson with bigger point, otherwise return the lesson with same point
        Lesson? nextLesson;
        if (curSkill >= SkillLevel.Advanced)
        {
            nextLesson = _course.Lessons.FirstOrDefault(l => l.Point > curLessonPoint);
        }
        else
        {
            nextLesson = _course.Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id > curLessonId)
                         ?? _course.Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id == curLessonId);
        }

        // get next lesson of the current level
        if (nextLesson != null)
        {
            return (nextLesson, nextLesson.PracticeText);
        }

        return (null, "Congratulation, You have completed all lessons in this course.");
    }

    public void CheckPracticeResult(int courseId, int lessonId, TypingStats stats)
    {
        if (_course.Id != courseId)
        {
            throw new ArgumentOutOfRangeException(nameof(courseId));
        }

        if (_course.Lessons.All(l => l.Id != lessonId))
        {
            throw new ArgumentOutOfRangeException(nameof(lessonId));
        }

        ArgumentNullException.ThrowIfNull(stats, nameof(stats));

        _account.Progress.Add(new LearningProgress
        {
            CourseId = courseId,
            LessonId = lessonId,
            Stats = stats,
            FinishDate = DateTime.Now
        });

    }
}