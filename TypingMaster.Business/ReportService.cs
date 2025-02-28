using Serilog;
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
                return 0;
            }
            return k.PressDuration / k.CorrectCount;
        }).ToList();
        var latency = historyKeyStats.Values.Select(k =>
        {
            if (k.CorrectCount == 0)
            {
                return 0;
            }

            return k.Latency / k.CorrectCount;
        }).ToList();
        var accuracies = historyKeyStats.Values.Select(k =>
        {
            if (k.TypingCount == 0)
            {
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

        }
        return stats;
    }
}