using Serilog;
using System.Globalization;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class ReportService(ILogger logger) : ServiceBase(logger), IReportService
{
    public IEnumerable<string> GetKeyLabels(PracticeLog history)
    {
        var historyKeyStats = history.KeyStats;
        var keys = historyKeyStats.Values.Select(x => x.Key.ToString()).ToList();
        return keys;
    }

    public Dictionary<string, IEnumerable<double>> GetKeyStats(PracticeLog history, bool includeLastSession = false)
    {
        var historyKeyStats = history.KeyStats;
        var typeSpeed = historyKeyStats.Values.Select(k =>
        {
            if (k.CorrectCount == 0)
            {
                ProcessResult.AddInformation(NoCorrectKeyPressCount);
                return 0;
            }
            return k.PressDuration / k.CorrectCount;
        }).ToList();
        var latency = historyKeyStats.Values.Select(k =>
        {
            if (k.CorrectCount == 0)
            {
                ProcessResult.AddInformation(NoCorrectKeyPressCount);
                return 0;
            }

            return k.Latency / k.CorrectCount;
        }).ToList();
        var accuracies = historyKeyStats.Values.Select(k =>
        {
            if (k.TypingCount == 0)
            {
                ProcessResult.AddInformation(NoKeyPressCount);
                return 0;
            }

            return (double)k.CorrectCount / k.TypingCount * 100;
        }).ToList();
        var stats = new Dictionary<string, IEnumerable<double>>
        {
            { "typeSpeed", typeSpeed },
            { "latency", latency },
            { "accuracy", accuracies }
        };

        if (includeLastSession)
        {
            // todo: add last session stats
        }
        return stats;
    }

    public IEnumerable<ProgressRecord> GetProgressRecords(PracticeLog history, ICourse course, TrainingType type)
    {
        var dataList = new List<ProgressRecord>();
        foreach (var item in history.PracticeStats)
        {
            switch (type)
            {
                case TrainingType.Course:
                    if (item.Type == TrainingType.Course)
                    {
                        var record = new ProgressRecord
                        {
                            Type = TrainingType.Course.ToString(),
                            Name = course.Name,
                            Date = item.StartTime?.ToString() ?? DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            GoodWpmKeys = CalculateGoodWpmKeys(item.KeyEvents),
                            OverallAccuracy = item.Accuracy, // Implement this method
                            OverallSpeed = item.Wpm, // Implement this method
                            BreakdownLetter = CalculateBreakdownLetter(item.KeyEvents, item.Wpm),
                            BreakdownNumber = CalculateBreakdownNumber(item.KeyEvents, item.Wpm),
                            BreakdownSymbol = CalculateBreakdownSymbol(item.KeyEvents, item.Wpm)
                        };

                        dataList.Add(record);
                    }

                    break;
                case TrainingType.AllKeysTest:
                case TrainingType.SpeedTest:

                    if (item.Type is TrainingType.AllKeysTest or TrainingType.SpeedTest )
                    {
                        var record = new ProgressRecord
                        {
                            Type = TrainingType.Course.ToString(),
                            Name = course.Name,
                            Date = item.StartTime?.ToString() ?? DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            GoodWpmKeys = CalculateGoodWpmKeys(item.KeyEvents),
                            OverallAccuracy = item.Accuracy, // Implement this method
                            OverallSpeed = item.Wpm, // Implement this method
                            BreakdownLetter = CalculateBreakdownLetter(item.KeyEvents, item.Wpm),
                            BreakdownNumber = CalculateBreakdownNumber(item.KeyEvents, item.Wpm),
                            BreakdownSymbol = CalculateBreakdownSymbol(item.KeyEvents, item.Wpm)
                        };

                        dataList.Add(record);
                    }

                    break;
            }
        }

        return dataList;
    }

    private static int CalculateGoodWpmKeys(Queue<KeyEvent> keyEvents)
    {
        var count = 0;
        if (keyEvents.Count < 2) return count; // Need at least two events to calculate speed

        var keyGroups = keyEvents.GroupBy(k => k.Key).ToList();

        foreach (var keyGroup in keyGroups)
        {
            var keyEventList = keyGroup.ToList();

            var totalTime = (keyEventList.Last().KeyUpTime - keyEventList.First().KeyDownTime).TotalMinutes;

            if (totalTime <= 0)
            {
                continue;
            }

            var totalLetters = keyEventList.Count;
            var wpm = (totalLetters / 5.0) / totalTime;

            if (wpm > 20)
            {
                count++;
            }
        }

        return count;
    }

    private static int CalculateBreakdownLetter(Queue<KeyEvent> keyEvents, int overallSpeed)
    {
        if (keyEvents.Count == 0 && keyEvents.All(k => char.IsLetter(k.Key)))
        {
            return overallSpeed;
        }

        // Filter key events to include only letters
        var letterEvents = keyEvents.Where(k => char.IsLetter(k.Key)).ToList();

        return letterEvents.Count == 0 ? 0 : CalculateKeySpeed(letterEvents);
    }

    private static int CalculateBreakdownNumber(Queue<KeyEvent> keyEvents, int overallSpeed)
    {
        if (keyEvents.Count == 0 && keyEvents.All(k => char.IsNumber(k.Key)))
        {
            return overallSpeed;
        }

        // Filter key events to include only number
        var numberEvents = keyEvents.Where(k => char.IsNumber(k.Key)).ToList();

        return numberEvents.Count == 0 ? 0 : CalculateKeySpeed(numberEvents);
    }

    private static int CalculateBreakdownSymbol(Queue<KeyEvent> keyEvents, int overallSpeed)
    {
        if (keyEvents.Count == 0 && keyEvents.All(k => char.IsSymbol(k.Key)))
        {
            return overallSpeed;
        }

        // Filter key events to include only symbols
        var symbolEvents = keyEvents.Where(k => char.IsSymbol(k.Key)).ToList();

        return symbolEvents.Count == 0 ? 0 : CalculateKeySpeed(symbolEvents);
    }

    private static int CalculateKeySpeed(List<KeyEvent> keyEvents)
    {
        // Calculate the total time taken for these key events
        var totalTime = (keyEvents.Last().KeyUpTime - keyEvents.First().KeyDownTime).TotalMinutes;

        if (totalTime <= 0)
        {
            return 0;
        }

        // Calculate WPM (Words Per Minute)
        // Assuming 1 word = 5 letters
        var totalLetters = keyEvents.Count;
        var wpm = (totalLetters / 5.0) / totalTime;
        return (int)wpm;
    }
}