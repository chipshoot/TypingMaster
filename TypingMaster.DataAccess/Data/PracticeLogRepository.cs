using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class PracticeLogRepository(ApplicationDbContext context, Serilog.ILogger logger)
    : RepositoryBase(logger), IPracticeLogRepository
{
    public async Task<PracticeLogDao?> GetPracticeLogByIdAsync(int id)
    {
        try
        {
            var practiceLog = await context.PracticeLogs
                .Include(p => p.PracticeStats)
                .FirstOrDefaultAsync(p => p.Id == id);

            return practiceLog;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<PracticeLogDao?> GetPracticeLogByAccountIdAsync(int accountId)
    {
        try
        {
            var practiceLog = await context.PracticeLogs
                .Include(p => p.PracticeStats)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);

            return practiceLog;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<PracticeLogDao?> CreatePracticeLogAsync(PracticeLogDao practiceLog)
    {
        try
        {
            // Verify that AccountId is set
            if (practiceLog.AccountId == 0)
            {
                ProcessResult.AddError("AccountId must be set when creating a practice log");
                return null;
            }

            // Verify that the account exists
            var accountExists = await context.Accounts.AnyAsync(a => a.Id == practiceLog.AccountId);
            if (!accountExists)
            {
                ProcessResult.AddError($"Account with ID {practiceLog.AccountId} not found");
                return null;
            }

            context.PracticeLogs.Add(practiceLog);
            await context.SaveChangesAsync();
            return practiceLog;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<PracticeLogDao?> UpdatePracticeLogAsync(PracticeLogDao practiceLog)
    {
        try
        {
            // Check if practice log exists
            var existingPracticeLog = await context.PracticeLogs
                .Include(p => p.PracticeStats)
                .FirstOrDefaultAsync(p => p.Id == practiceLog.Id);

            if (existingPracticeLog == null)
            {
                ProcessResult.AddError($"Practice log with ID {practiceLog.Id} not found");
                return null;
            }

            // Verify that AccountId matches and hasn't changed
            if (existingPracticeLog.AccountId != practiceLog.AccountId)
            {
                ProcessResult.AddError("Cannot change the AccountId of a practice log");
                return null;
            }

            // Update PracticeLog properties
            context.Entry(existingPracticeLog).CurrentValues.SetValues(practiceLog);

            // Update KeyStatsJson dictionary
            existingPracticeLog.KeyStatsJson = practiceLog.KeyStatsJson;

            // Handle DrillStats collection - we don't update them here, use AddDrillStatAsync instead

            await context.SaveChangesAsync();
            return existingPracticeLog;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }


    public async Task<IEnumerable<DrillStatsDao>> AddBulkDrillStatsAsync(int practiceLogId, IEnumerable<DrillStatsDao> drillStats)
    {
        try
        {
            // Verify the practice log exists
            var practiceLogExists = await context.PracticeLogs.AnyAsync(p => p.Id == practiceLogId);
            if (!practiceLogExists)
            {
                ProcessResult.AddError($"Practice log with ID {practiceLogId} not found");
                return new List<DrillStatsDao>();
            }

            var drillStatsList = drillStats.ToList();
            foreach (var drillStat in drillStatsList)
            {
                drillStat.PracticeLogId = practiceLogId;
                context.DrillStats.Add(drillStat);
            }

            await context.SaveChangesAsync();
            return drillStatsList;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return new List<DrillStatsDao>();
        }
    }

    public async Task<DrillStatsDao?> AddDrillStatAsync(int practiceLogId, DrillStatsDao drillStat)
    {
        try
        {
            // Verify the practice log exists
            var practiceLog = await context.PracticeLogs
                .Include(p => p.PracticeStats)
                .FirstOrDefaultAsync(p => p.Id == practiceLogId);

            if (practiceLog == null)
            {
                ProcessResult.AddError($"Practice log with ID {practiceLogId} not found");
                return null;
            }

            // Set the practice log ID on the drill stat
            drillStat.PracticeLogId = practiceLogId;

            // Add the drill stat
            context.DrillStats.Add(drillStat);

            await context.SaveChangesAsync();
            return drillStat;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<bool> CanDeletePracticeLogAsync(int id)
    {
        try
        {
            // Check if practice log exists
            var practiceLog = await context.PracticeLogs
                .FirstOrDefaultAsync(p => p.Id == id);

            if (practiceLog == null)
            {
                ProcessResult.AddInformation($"Practice log with ID {id} not found");
                return false;
            }

            // Check if this practice log is associated with an active account
            var isAssociatedWithAccount = await context.Accounts
                .AnyAsync(a => a.History.Id == id && !a.IsDeleted);

            if (isAssociatedWithAccount)
            {
                ProcessResult.AddInformation("Cannot delete practice log that belongs to an active account");
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return false;
        }
    }

    public async Task<bool> DeletePracticeLogAsync(int id)
    {
        try
        {
            // First check if deletion is allowed
            if (!await CanDeletePracticeLogAsync(id))
            {
                return false;
            }

            // Find the practice log
            var practiceLog = await context.PracticeLogs
                .FirstOrDefaultAsync(p => p.Id == id);

            if (practiceLog == null)
            {
                ProcessResult.AddInformation($"Practice log with ID {id} not found");
                return false;
            }

            // Delete the practice log (child drill stats will be cascade deleted)
            context.PracticeLogs.Remove(practiceLog);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return false;
        }
    }
}