using System.Text.Json;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Core.Utility;
using ILogger = Serilog.ILogger;

namespace TypingMaster.Client.Services;

public class TypingTrainer(IPracticeLogWebService practiceService, ILogger logger) : ITypingTrainer
{
    private const string NoCourse = "Cannot find course.";
    private const string NoPracticeLog = "Cannot find practice log.";
    private const string CourseNotMatch = "Course of History does not match Account's current course";
    private const string NoLesson = "Cannot find lesson.";
    private const string NotSupportedType = "Not supported training type found";

    private readonly ILogger _logger = logger ?? throw new ArgumentException(nameof(logger));
    private Account? _account;
    private CourseDto? _course;
    private readonly TrainingType _trainingType = TrainingType.Course;
    private PracticeLog? _practiceLog;

    public void SetupTrainer(Account account, CourseDto course)
    {
        Guard.AgainstNull(account, nameof(account));
        Guard.AgainstNull(course, nameof(course));

        _account = account;

        _practiceLog = _account.History;
        switch (_practiceLog)
        {
            case null:
                ProcessResult.AddError(NoPracticeLog);
                throw new InvalidOperationException(NoPracticeLog);
        }

        _course = course;
    }

    public Account? Account => _account;

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

            ConvertKeyEventToKeyStats(stats.KeyEvents);
            _practiceLog.PracticeDuration += (long)(stats.FinishTime?.Subtract(stats.StartTime ?? DateTime.Now) ?? TimeSpan.Zero).TotalSeconds;
            _practiceLog.PracticeStats.Add(stats);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            _logger.Error(ex, "Error checking practice result");
        }
    }


    public async Task<bool> SavePracticeHistoryAsync(DrillStats stats)
    {
        try
        {
            // First update the in-memory account data
            CheckPracticeResult(stats);

            if (_account == null)
            {
                ProcessResult.AddError("Cannot save practice history: No account is set");
                return false;
            }

            // Now persist the updated account to the database
            if (_practiceLog == null)
            {
                ProcessResult.AddError("Null practiceLog found");
                return false;
            }

            var statsToSaveLst = _practiceLog.PracticeStats.Where(d => d.Id <= 0).Select(d=>d).ToList();
            if (statsToSaveLst.Count == 0)
            {
                return true;
            }

            var tempList = _practiceLog.PracticeStats;

            // only save the stats that are not already saved (id <= 0)
            _practiceLog.PracticeStats = statsToSaveLst;
            var updatedLog = await practiceService.UpdatePracticeLog(_practiceLog);
            if (updatedLog == null)
            {
                ProcessResult.AddError("Failed to save drill stat");
                return false;
            }

            var newId= updatedLog.PracticeStats.FirstOrDefault()?.Id ?? 0;
            var newStat = tempList.FirstOrDefault(d => d.Id == 0);
            if (newStat != null)
            {
                newStat.Id = newId;
            }

            _practiceLog.PracticeStats = tempList; // restore the original list
            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            _logger.Error(ex, "Error saving practice history");
            return false;
        }
    }

    public void ConvertKeyEventToKeyStats(Queue<KeyEvent> keyEvents)
    {
        if (_practiceLog == null || keyEvents == null || keyEvents.Count == 0)
        {
            return;
        }

        var keyStats = _practiceLog.KeyStats;
        var statsDictionary = new Dictionary<char, KeyStats>(keyEvents.Count);

        foreach (var keyEvent in keyEvents)
        {
            // Skip null characters or other problematic control characters
            if (keyEvent.Key == '\0' || char.IsControl(keyEvent.Key))
            {
                continue;
            }

            if (!statsDictionary.TryGetValue(keyEvent.Key, out var stats))
            {
                stats = new KeyStats
                {
                    Key = keyEvent.Key.ToString(),
                    TypingCount = 0,
                    CorrectCount = 0,
                    PressDuration = 0,
                    Latency = 0
                };
                statsDictionary[keyEvent.Key] = stats;
            }

            stats.TypingCount++;
            if (keyEvent.IsCorrect)
            {
                stats.CorrectCount++;
            }

            stats.PressDuration += (keyEvent.KeyUpTime - keyEvent.KeyDownTime).TotalMilliseconds;
            stats.Latency += keyEvent.Latency;
        }

        // Update the practice log with the new stats
        foreach (var kvp in statsDictionary)
        {
            var stats = kvp.Value;
            if (stats.TypingCount > 0)
            {
                stats.Latency /= stats.TypingCount;
            }
            keyStats[kvp.Key] = stats;
        }

        _practiceLog.KeyStats = keyStats;

        if (logger.IsEnabled(Serilog.Events.LogEventLevel.Debug))
        {
            var jsonString = JsonSerializer.Serialize(keyEvents);
            logger.Debug($"events:{jsonString}");
            jsonString = JsonSerializer.Serialize(keyStats);
            logger.Debug($"stats{jsonString}");
        }
    }

    public ProcessResult ProcessResult { get; set; } = new(logger);
}