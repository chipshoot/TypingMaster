using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Contract;

public interface IPracticeLogService
{
    /// <summary>
    /// Get a practice log by its ID
    /// </summary>
    Task<PracticeLog?> GetPracticeLogById(int id);

    /// <summary>
    /// Get a practice log by account ID
    /// </summary>
    Task<PracticeLog?> GetPracticeLogByAccountId(int accountId);

    /// <summary>
    /// Create a new practice log
    /// </summary>
    Task<PracticeLog?> CreatePracticeLog(PracticeLog practiceLog);

    /// <summary>
    /// Update an existing practice log
    /// </summary>
    Task<PracticeLog?> UpdatePracticeLog(PracticeLog practiceLog);

    /// <summary>
    /// Add a drill stat to a practice log
    /// </summary>
    Task<DrillStats?> AddDrillStat(int practiceLogId, DrillStats drillStat);

    /// <summary>
    /// Gets a paginated list of drill stats for a specific practice log.
    /// </summary>
    /// <param name="practiceLogId">The ID of the practice log.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="sortByNewest">Indicates whether to sort the results by newest first. Defaults to true.</param>
    /// <returns>A paginated result containing the drill stats.</returns>
    Task<PagedResult<DrillStats>> GetPaginatedDrillStatsByPracticeLogIdAsync(
        int practiceLogId,
        int page = 1,
        int pageSize = 10,
        bool sortByNewest = true);

    /// <summary>
    /// Update an existing drill stat
    /// </summary>
    Task<DrillStats?> UpdateDrillStat(DrillStats drillStat);

    /// <summary>
    /// Delete a drill stat
    /// </summary>
    Task<bool> DeleteDrillStat(int id);

    ProcessResult ProcessResult { get; set; }
}