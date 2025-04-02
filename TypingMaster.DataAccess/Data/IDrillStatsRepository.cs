using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.DataAccess.Data;

public interface IDrillStatsRepository
{
    /// <summary>
    /// Get a drill stat by its ID
    /// </summary>
    /// <param name="id">The ID of the drill stat to retrieve</param>
    /// <returns>The drill stat if found, null otherwise</returns>
    Task<DrillStatsDao?> GetDrillStatByIdAsync(int id);

    /// <summary>
    /// Get all drill stats for a practice log
    /// </summary>
    /// <param name="practiceLogId">The ID of the practice log</param>
    /// <param name="page">The page number of dataset</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="sortByNewest">Sort by date</param>
    /// <returns>Collection of drill stats for the practice log</returns>
    Task<(IEnumerable<DrillStatsDao> Items, int TotalCount)> GetPaginatedDrillStatsByPracticeLogIdAsync(
        int practiceLogId,
        int page, 
        int pageSize,
        bool sortByNewest);

    /// <summary>
    /// Create a new drill stat
    /// </summary>
    /// <param name="drillStat">The drill stat to create</param>
    /// <returns>The created drill stat with ID populated</returns>
    Task<DrillStatsDao?> CreateDrillStatAsync(DrillStatsDao drillStat);

    /// <summary>
    /// Update an existing drill stat
    /// </summary>
    /// <param name="drillStat">The drill stat with updated values</param>
    /// <returns>The updated drill stat if successful, null otherwise</returns>
    Task<DrillStatsDao?> UpdateDrillStatAsync(DrillStatsDao drillStat);

    /// <summary>
    /// Delete a drill stat
    /// </summary>
    /// <param name="id">The ID of the drill stat to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteDrillStatAsync(int id);

    /// <summary>
    /// Process result for tracking errors and operation status
    /// </summary>
    ProcessResult ProcessResult { get; protected set; }
}