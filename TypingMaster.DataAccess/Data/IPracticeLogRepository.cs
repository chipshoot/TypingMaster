using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.DataAccess.Data;

public interface IPracticeLogRepository
{
    /// <summary>
    /// Get a practice log by its ID
    /// </summary>
    /// <param name="id">The ID of the practice log to retrieve</param>
    /// <returns>The practice log if found, null otherwise</returns>
    Task<PracticeLogDao?> GetPracticeLogByIdAsync(int id);

    /// <summary>
    /// Get a practice log by account ID
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The practice log if found, null otherwise</returns>
    Task<PracticeLogDao?> GetPracticeLogByAccountIdAsync(int accountId);

    /// <summary>
    /// Create a new practice log
    /// </summary>
    /// <param name="practiceLog">The practice log to create</param>
    /// <returns>The created practice log with ID populated</returns>
    Task<PracticeLogDao?> CreatePracticeLogAsync(PracticeLogDao practiceLog);

    /// <summary>
    /// Update an existing practice log
    /// </summary>
    /// <param name="practiceLog">The practice log with updated values</param>
    /// <returns>The updated practice log if successful, null otherwise</returns>
    Task<PracticeLogDao?> UpdatePracticeLogAsync(PracticeLogDao practiceLog);

    /// <summary>
    /// Add a drill stat to a practice log
    /// </summary>
    /// <param name="practiceLogId">The ID of the practice log</param>
    /// <param name="drillStat">The drill stat to add</param>
    /// <returns>The added drill stat with ID populated</returns>
    Task<DrillStatsDao?> AddDrillStatAsync(int practiceLogId, DrillStatsDao drillStat);


    /// <summary>
    /// Add multiple drill stats to a practice log in bulk
    /// </summary>
    /// <param name="practiceLogId">The ID of the practice log</param>
    /// <param name="drillStats">The drill stats to add</param>
    /// <returns>The added drill stats with IDs populated</returns>
    Task<IEnumerable<DrillStatsDao>> AddBulkDrillStatsAsync(int practiceLogId, IEnumerable<DrillStatsDao> drillStats);

    /// <summary>
    /// Check if a practice log can be safely deleted
    /// </summary>
    /// <param name="id">The ID of the practice log to check</param>
    /// <returns>True if the practice log can be deleted, false otherwise</returns>
    Task<bool> CanDeletePracticeLogAsync(int id);

    /// <summary>
    /// Delete a practice log
    /// </summary>
    /// <param name="id">The ID of the practice log to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeletePracticeLogAsync(int id);

    /// <summary>
    /// Process result for tracking errors and operation status
    /// </summary>
    ProcessResult ProcessResult { get; protected set; }
}