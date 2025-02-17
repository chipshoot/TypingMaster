using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class TypingTrainer(Account account, ILogger logger) : ITypingTrainer
{
    private readonly Account _account = account ?? throw new ArgumentException(nameof(account));
    private readonly ICourse _course = account.CurrentCourse ?? throw new ArgumentException(nameof(account.CurrentCourse));
    private readonly PracticeLog _practiceLog = account.History ?? throw new ArgumentException(nameof(account.History));
    private readonly ILogger _logger = logger ?? throw new ArgumentException(nameof(logger));

    public string GetPracticeText(int lessonId)
    {
        return _course.Lessons.FirstOrDefault(x => x.Id == lessonId)?.PracticeText ?? string.Empty;
    }

    public string GetPracticeText()
    {
        if (_account.History == null) throw new InvalidOperationException("Practice history not found.");
        if (_course.Lessons == null) throw new InvalidOperationException("Course lesson not found.");

        //PracticeLog currentProgress;

        //if (_account.Progress.Any())
        //{
        //    currentProgress = _account.Progress.Last();
        //}
        //else
        //{
        //    currentProgress = new LearningProgress
        //    {
        //        CourseId = _account.CurrentCourse.Id,
        //        LessonId = 0,
        //        Stats = null
        //    };
        //}
        //var curLessonId = currentProgress.LessonId;
        //var curLessonPoint = _course.Lessons.FirstOrDefault(x => x.Id == curLessonId)?.Point ?? 1;
        //var curSkill = currentProgress.Stats?.GetSkillLevel() ?? SkillLevel.Beginner;

        // if current skill level above advanced, return the lesson with bigger point, otherwise return the lesson with same point
        //Lesson? nextLesson;
        //if (curSkill >= SkillLevel.Advanced)
        //{
        //    nextLesson = _course.Lessons.FirstOrDefault(l => l.Point > curLessonPoint);
        //}
        //else
        //{
        //    nextLesson = _course.Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id > curLessonId)
        //                 ?? _course.Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id == curLessonId);
        //}

        //// get next lesson of the current level
        //if (nextLesson != null)
        //{
        //    return (nextLesson, nextLesson.PracticeText);
        //}

        return "Congratulation, You have completed all lessons in this course.";
    }

    public void CheckPracticeResult(DrillStats stats)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            if (_course.Id != stats.CourseId)
            {
                throw new ArgumentOutOfRangeException(nameof(stats.CourseId));
            }

            if (_course.Lessons.All(l => l.Id != stats.LessonId))
            {
                throw new ArgumentOutOfRangeException(nameof(stats.LessonId));
            }

            //_account.Progress.Add(new LearningProgress
            //{
            //    CourseId = courseId,
            //    LessonId = lessonId,
            //    Stats = stats,
            //    FinishDate = DateTime.Now
            //});

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking practice result");
        }
    }
}