using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface IReportService
{
    IEnumerable<string> GetKeyLabels(PracticeLog history);

    Dictionary<string, IEnumerable<double>> GetKeyStats(PracticeLog history, bool includeLastSession);

    IEnumerable<ProgressRecord> GetProgressRecords(PracticeLog history, ICourse course);
}