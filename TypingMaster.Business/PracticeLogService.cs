using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class PracticeLogService(
    IPracticeLogRepository practiceLogRepository,
    IDrillStatsRepository drillStatsRepository,
    IMapper mapper,
    ILogger logger) : ServiceBase(logger), IPracticeLogService
{
    public async Task<PracticeLog?> GetPracticeLogById(int id)
    {
        try
        {
            var practiceLogDao = await practiceLogRepository.GetPracticeLogByIdAsync(id);
            if (practiceLogDao == null)
            {
                return null;
            }

            return mapper.Map<PracticeLog>(practiceLogDao);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<PracticeLog?> GetPracticeLogByAccountId(int accountId)
    {
        try
        {
            var practiceLogDao = await practiceLogRepository.GetPracticeLogByAccountIdAsync(accountId);
            if (practiceLogDao == null)
            {
                return null;
            }

            return mapper.Map<PracticeLog>(practiceLogDao);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<PracticeLog?> CreatePracticeLog(PracticeLog practiceLog)
    {
        try
        {
            // Store drill stats temporarily
            var drillStats = practiceLog.PracticeStats?.ToList() ?? [];

            // Validate AccountId
            if (practiceLog.AccountId <= 0)
            {
                ProcessResult.AddError("AccountId must be greater than 0");
                return null;
            }

            // Create a new practice log without drill stats
            var practiceLogToCreate = new PracticeLog
            {
                AccountId = practiceLog.AccountId,
                Id = practiceLog.Id,
                CurrentCourseId = practiceLog.CurrentCourseId,
                CurrentLessonId = practiceLog.CurrentLessonId,
                KeyStats = practiceLog.KeyStats,
                PracticeDuration = practiceLog.PracticeDuration
            };

            // Create practice log first
            var practiceLogDao = mapper.Map<PracticeLogDao>(practiceLogToCreate);
            var createdPracticeLogDao = await practiceLogRepository.CreatePracticeLogAsync(practiceLogDao);
            if (createdPracticeLogDao == null)
            {
                ProcessResult.AddError("Failed to create practice log");
                return null;
            }

            // Add drill stats one by one using the service method
            var updatedDrillStats = new List<DrillStats>();
            foreach (var drillStat in drillStats)
            {
                var addedDrillStat = await AddDrillStat(createdPracticeLogDao.Id, drillStat);
                if (addedDrillStat == null)
                {
                    logger.Warning("Failed to add drill stat during practice log creation. PracticeLogId: {PracticeLogId}, DrillStat: {@DrillStat}",
                        createdPracticeLogDao.Id, drillStat);
                }
                else
                {
                    updatedDrillStats.Add(addedDrillStat);
                }
            }

            // Get the complete practice log with all drill stats
            var finalPracticeLogDao = await practiceLogRepository.GetPracticeLogByIdAsync(createdPracticeLogDao.Id);
            if (finalPracticeLogDao == null) return null;

            var result = mapper.Map<PracticeLog>(finalPracticeLogDao);
            // Ensure we use our updated drill stats with proper IDs
            result.PracticeStats = updatedDrillStats;
            return result;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<PracticeLog?> UpdatePracticeLog(PracticeLog practiceLog)
    {
        try
        {
            // Get existing practice log to compare drill stats
            var existingPracticeLogDao = await practiceLogRepository.GetPracticeLogByIdAsync(practiceLog.Id);
            if (existingPracticeLogDao == null)
            {
                ProcessResult.AddError($"Practice log with ID {practiceLog.Id} not found");
                return null;
            }

            // Store drill stats temporarily
            var newDrillStats = practiceLog.PracticeStats?.ToList() ?? [];
            var existingDrillStatIds = existingPracticeLogDao.PracticeStats?.Select(ds => ds.Id).ToHashSet() ?? [];

            // Create a practice log without drill stats for updating
            var practiceLogToUpdate = new PracticeLog
            {
                AccountId = practiceLog.AccountId,
                Id = practiceLog.Id,
                CurrentCourseId = practiceLog.CurrentCourseId,
                CurrentLessonId = practiceLog.CurrentLessonId,
                KeyStats = practiceLog.KeyStats,
                PracticeDuration = practiceLog.PracticeDuration
            };

            // Update practice log first
            var practiceLogDao = mapper.Map<PracticeLogDao>(practiceLogToUpdate);
            var updatedPracticeLogDao = await practiceLogRepository.UpdatePracticeLogAsync(practiceLogDao);
            if (updatedPracticeLogDao == null)
            {
                ProcessResult.AddError("Failed to update practice log");
                return null;
            }

            // Handle drill stats using service methods
            var updatedDrillStats = new List<DrillStats>();
            foreach (var drillStat in newDrillStats)
            {
                if (drillStat.Id == 0 || !existingDrillStatIds.Contains(drillStat.Id))
                {
                    // New drill stat - add it using service method
                    var addedDrillStat = await AddDrillStat(updatedPracticeLogDao.Id, drillStat);
                    if (addedDrillStat == null)
                    {
                        logger.Warning("Failed to add new drill stat during practice log update. PracticeLogId: {PracticeLogId}, DrillStat: {@DrillStat}",
                            updatedPracticeLogDao.Id, drillStat);
                    }
                    else
                    {
                        updatedDrillStats.Add(addedDrillStat);
                    }
                }
                else
                {
                    // Existing drill stat - update it using service method
                    drillStat.PracticeLogId = updatedPracticeLogDao.Id; // Ensure correct practice log ID
                    var updatedDrillStat = await UpdateDrillStat(drillStat);
                    if (updatedDrillStat == null)
                    {
                        logger.Warning("Failed to update drill stat during practice log update. PracticeLogId: {PracticeLogId}, DrillStat: {@DrillStat}",
                            updatedPracticeLogDao.Id, drillStat);
                    }
                    else
                    {
                        updatedDrillStats.Add(updatedDrillStat);
                    }
                }
            }

            // Get the complete updated practice log with all drill stats
            var finalPracticeLogDao = await practiceLogRepository.GetPracticeLogByIdAsync(updatedPracticeLogDao.Id);
            if (finalPracticeLogDao == null) return null;

            var result = mapper.Map<PracticeLog>(finalPracticeLogDao);
            // Ensure we use our updated drill stats with proper IDs
            result.PracticeStats = updatedDrillStats;
            return result;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<DrillStats?> AddDrillStat(int practiceLogId, DrillStats drillStat)
    {
        try
        {
            // First check if practice log exists
            var practiceLog = await practiceLogRepository.GetPracticeLogByIdAsync(practiceLogId);
            if (practiceLog == null)
            {
                ProcessResult.AddError($"Practice log with ID {practiceLogId} not found");
                return null;
            }

            // Map and add the drill stat
            var drillStatDao = mapper.Map<DrillStatsDao>(drillStat);
            drillStatDao.PracticeLogId = practiceLogId;

            var addedDrillStatDao = await drillStatsRepository.CreateDrillStatAsync(drillStatDao);
            if (addedDrillStatDao == null)
            {
                ProcessResult.AddError("Failed to add drill stat");
                return null;
            }

            // Update the original drillStat object with the new data
            drillStat.Id = addedDrillStatDao.Id;
            drillStat.PracticeLogId = addedDrillStatDao.PracticeLogId;

            // Return the updated original object instead of creating a new mapped one
            return drillStat;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<PagedResult<DrillStats>> GetPaginatedDrillStatsByPracticeLogIdAsync(
        int practiceLogId,
        int page = 1,
        int pageSize = 10,
        bool sortByNewest = true)
    {

        var pagedResult = new PagedResult<DrillStats>
        {
            Page = page,
            PageSize = pageSize
        };

        try
        {
            // Use the repository to get paginated data
            var (daoItems, totalCount) = await drillStatsRepository.GetPaginatedDrillStatsByPracticeLogIdAsync(
                practiceLogId, page, pageSize, sortByNewest);

            // Set pagination metadata
            pagedResult.TotalCount = totalCount;
            pagedResult.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Map DAOs to domain models
            var items = new List<DrillStats>();
            foreach (var dao in daoItems)
            {
                var drillStats = mapper.Map<DrillStats>(dao);
                items.Add(drillStats);
            }

            pagedResult.Items = items;
            pagedResult.Success = true;

            return pagedResult;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return pagedResult;
        }
    }

    public async Task<DrillStats?> UpdateDrillStat(DrillStats drillStat)
    {
        try
        {
            var drillStatDao = mapper.Map<DrillStatsDao>(drillStat);
            var updatedDrillStatDao = await drillStatsRepository.UpdateDrillStatAsync(drillStatDao);
            if (updatedDrillStatDao == null)
            {
                ProcessResult.AddError("Failed to update drill stat");
                return null;
            }

            return mapper.Map<DrillStats>(updatedDrillStatDao);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<bool> DeleteDrillStat(int id)
    {
        try
        {
            return await drillStatsRepository.DeleteDrillStatAsync(id);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }
}