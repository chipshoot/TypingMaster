using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.Business.Contract;

public interface IReportService
{
    IEnumerable<string> GetKeyLabels(PracticeLog history);

    Dictionary<string, IEnumerable<double>> GetKeyStats(PracticeLog history, bool includeLastSession);

    Task<PagedResult<ProgressRecord>> GetProgressRecords(
        PracticeLog history,
        ICourse course, 
        TrainingType type, 
        int page = 1,
        int pageSize = 10, 
        bool sortByNewest = true);

    ProcessResult ProcessResult { get; set; }
}