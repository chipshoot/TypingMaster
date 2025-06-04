using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class PracticeLogService(
    IPracticeLogRepository practiceLogRepository,
    IDrillStatsRepository drillStatsRepository,
    IMapper mapper,
    ILogger logger) : IPracticeLogService
{
    public ProcessResult ProcessResult { get; set; } = new(logger);

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
            // Validate AccountId
            if (practiceLog.AccountId <= 0)
            {
                ProcessResult.AddError("AccountId must be greater than 0");
                return null;
            }

            // Store drill stats temporarily
            var drillStats = practiceLog.PracticeStats?.ToList() ?? [];

            // Create a new practice log without drill stats first
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

            // Add drill stats in bulk if any exist
            if (drillStats.Any())
            {
                var drillStatDaos = drillStats.Select(mapper.Map<DrillStatsDao>);
                var addedDrillStatDaos = await practiceLogRepository.AddBulkDrillStatsAsync(
                    createdPracticeLogDao.Id, drillStatDaos);

                if (!addedDrillStatDaos.Any() && drillStats.Any())
                {
                    logger.Warning("Failed to add drill stats during practice log creation. PracticeLogId: {PracticeLogId}",
                        createdPracticeLogDao.Id);
                }
            }

            // Get the complete practice log with all drill stats
            var finalPracticeLogDao = await practiceLogRepository.GetPracticeLogByIdAsync(createdPracticeLogDao.Id);
            if (finalPracticeLogDao == null) return null;

            return mapper.Map<PracticeLog>(finalPracticeLogDao);
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
            // Validate input
            if (practiceLog.AccountId <= 0)
            {
                ProcessResult.AddError("AccountId must be greater than 0");
                return null;
            }

            // Separate new and existing drill stats
            var allDrillStats = practiceLog.PracticeStats?.ToList() ?? [];
            var newDrillStats = allDrillStats.Where(s => s.Id == 0).ToList();
            var existingDrillStats = allDrillStats.Where(s => s.Id > 0).ToList();


            // Create practice log DAO for update (without drill stats)
            var practiceLogToUpdate = new PracticeLog
            {
                AccountId = practiceLog.AccountId,
                Id = practiceLog.Id,
                CurrentCourseId = practiceLog.CurrentCourseId,
                CurrentLessonId = practiceLog.CurrentLessonId,
                KeyStats = practiceLog.KeyStats,
                PracticeDuration = practiceLog.PracticeDuration
            };

            var createdDrillStats = await AddDrillStats(practiceLog.Id, newDrillStats);
            if (createdDrillStats == null)
            {
                ProcessResult.AddError("Failed to update practice log");
                return null;
            }

            var updatedDrillStats = await UpdateDrillStats(existingDrillStats);

            var practiceLogDao = mapper.Map<PracticeLogDao>(practiceLogToUpdate);
            var updatedPracticeLogDao = await practiceLogRepository.UpdatePracticeLogAsync(practiceLogDao);

            if (updatedPracticeLogDao == null)
            {
                ProcessResult.AddError("Failed to update practice log");
                return null;
            }

            // Map back to domain model
            var result = mapper.Map<PracticeLog>(updatedPracticeLogDao);
            result.PracticeStats = createdDrillStats;
            return result;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }


    private async Task<List<DrillStats>?> AddDrillStats(int practiceLogId, List<DrillStats> drillStats)
    {
        try
        {
            // Map and add the drill stat
            var drillStatDao = drillStats.Select(d =>
            {
                var dao = mapper.Map<DrillStatsDao>(d);
                dao.PracticeLogId = practiceLogId;
                return dao;
            }).ToList();

            var addedDrillStatDaos = await drillStatsRepository.BatchCreateDrillStatAsync(drillStatDao);
            if (addedDrillStatDaos == null)
            {
                ProcessResult.AddError("Failed to add drill stat");
                return null;
            }

            // Update the original drillStat object with the new data
            var addedDrillStats = addedDrillStatDaos.Select(mapper.Map<DrillStats>).ToList();

            // Return the updated original object instead of creating a new mapped one
            return addedDrillStats;
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

            var addedDrillStatDao = await drillStatsRepository.CreateDrillStatAsync(drillStatDao, false);
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

    public async Task<PagedResult<DrillStats>> GetPaginatedDrillStatsByPracticeLogId(
        int practiceLogId,
        int page = 1,
        int pageSize = 10,
        bool sortByNewest = true,
        TrainingType? type = null)
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
                practiceLogId, page, pageSize, sortByNewest, type);

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
            var updatedDrillStatDao = await drillStatsRepository.UpdateDrillStatAsync(drillStatDao, false);
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

    private async Task<List<DrillStats>?> UpdateDrillStats(List<DrillStats> drillStats)
    {
        try
        {
            if (drillStats.Count == 0)
            {
                return [];
            }

            var drillStatDaos = drillStats.Select(mapper.Map<DrillStatsDao>).ToList();
            var updatedDrillStatDaos = await drillStatsRepository.BatchUpdateDrillStatAsync(drillStatDaos);
            if (updatedDrillStatDaos == null)
            {
                ProcessResult.AddError("Failed to update drill stat");
                return null;
            }

            var updateDrillStats = updatedDrillStatDaos.Select(mapper.Map<DrillStats>).ToList();
            return updateDrillStats;
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