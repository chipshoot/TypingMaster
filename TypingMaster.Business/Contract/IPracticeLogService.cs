using TypingMaster.Core.Models;

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

    // Add to PracticeLogService.cs
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
}