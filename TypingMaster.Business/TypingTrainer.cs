using System.Text.Json;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Business.Utility;

namespace TypingMaster.Business;

public class TypingTrainer(ICourseService courseService, ILogger logger) : ITypingTrainer
{
    private const string NoCourse = "Cannot find course.";
    private const string NoPracticeLog = "Cannot find practice log.";
    private const string CourseNotMatch = "Course of History does not match Account's current course";
    private const string NoLesson = "Cannot find lesson.";
    private const string NotSupportedType = "Not supported training type found";

    private readonly ICourseService _courseService = courseService ?? throw new AbandonedMutexException(nameof(courseService));
    private readonly ILogger _logger = logger ?? throw new ArgumentException(nameof(logger));
    private Account? _account;
    private ICourse? _course;
    private readonly TrainingType _trainingType = TrainingType.Course;
    private PracticeLog? _practiceLog;

    public Account? Account
    {
        get => _account;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            SetupAccount(value).GetAwaiter().GetResult();
        }
    }

    public TrainingType TrainingType
    {
        get => _trainingType;
        set
        {
            if (value != _trainingType)
            {
                SetupCourse(value).GetAwaiter().GetResult();
            }
        }
    }

    public void CheckPracticeResult(DrillStats stats)
    {
        if (_course == null)
        {
            return;
        }

        if (_practiceLog == null)
        {
            return;
        }

        try
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            if (_course.Id != stats.CourseId)
            {
                ProcessResult.AddError(CourseNotMatch);
                throw new ArgumentOutOfRangeException(nameof(stats.CourseId));
            }

            if (_course.Lessons.All(l => l.Id != stats.LessonId))
            {
                ProcessResult.AddError(NoLesson);
                throw new ArgumentOutOfRangeException(nameof(stats.LessonId));
            }

            var practiceStatsList = _practiceLog.PracticeStats.ToList();
            practiceStatsList.Add(stats);
            _practiceLog.PracticeStats = practiceStatsList;
            ConvertKeyEventToKeyStats(stats.KeyEvents);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            _logger.Error(ex, "Error checking practice result");
        }
    }

    public void ConvertKeyEventToKeyStats(Queue<KeyEvent> keyEvents)
    {
        if (_practiceLog == null)
        {
            return;
        }

        var keyStats = _practiceLog.KeyStats;

        foreach (var keyEvent in keyEvents)
        {
            if (!keyStats.ContainsKey(keyEvent.Key))
            {
                keyStats[keyEvent.Key] = new KeyStats
                {
                    Key = keyEvent.Key.ToString(),
                    TypingCount = 0,
                    CorrectCount = 0,
                    PressDuration = 0,
                    Latency = 0
                };
            }

            var stats = keyStats[keyEvent.Key];
            stats.TypingCount++;
            if (keyEvent.IsCorrect)
            {
                stats.CorrectCount++;
            }
            stats.PressDuration += (keyEvent.KeyUpTime - keyEvent.KeyDownTime).TotalMilliseconds;
            stats.Latency += keyEvent.Latency;
        }

        // Calculate average latency
        foreach (var stats in keyStats.Values)
        {
            stats.Latency /= stats.TypingCount;
        }

        _practiceLog.KeyStats = keyStats;

        var jsonString = JsonSerializer.Serialize(keyEvents);
        _logger.Debug($"events:{jsonString}");
        jsonString = JsonSerializer.Serialize(keyStats);
        _logger.Debug($"stats{jsonString}");
    }

    public ProcessResult ProcessResult { get; set; } = new(logger);

    private async Task SetupAccount(Account account)
    {
        _account = account;

        switch (TrainingType)
        {
            case TrainingType.Course:
                _course = await _courseService.GetCourse(_account.CourseId);
                break;

            case TrainingType.Test:
                _course = await _courseService.GetCourse(_account.TestCourseId);
                break;
            case TrainingType.Game:
            default:
                ProcessResult.AddError(NotSupportedType);
                throw new Exception(NotSupportedType);
        }

        _practiceLog = _account.History;

        if (_practiceLog == null)
        {
            ProcessResult.AddError(NoPracticeLog);
            throw new InvalidOperationException(NoPracticeLog);
        }
    }

    private async Task SetupCourse(TrainingType type)
    {
        if (_account == null)
        {
            return;
        }

        switch (type)
        {
            case TrainingType.Course:
                _course = await _courseService.GetCourse(_account.CourseId);
                break;

            case TrainingType.Test:
                _course = await _courseService.GetCourse(_account.TestCourseId);
                break;
            case TrainingType.Game:
            default:
                ProcessResult.AddError(NotSupportedType);
                throw new Exception(NotSupportedType);
        }

        if (_course == null)
        {
            ProcessResult.AddError(NoCourse);
            throw new InvalidOperationException(NoCourse);
        }
    }
}