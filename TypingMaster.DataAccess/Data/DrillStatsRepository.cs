using System.Data;
using Microsoft.EntityFrameworkCore;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.DataAccess.Data;

public class DrillStatsRepository(ApplicationDbContext context, Serilog.ILogger logger) :  IDrillStatsRepository
{
    public async Task<DrillStatsDao?> GetDrillStatByIdAsync(int id)
    {
        try
        {
            var drillStat = await context.DrillStats
                .FirstOrDefaultAsync(d => d.Id == id);

            return drillStat;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<(IEnumerable<DrillStatsDao> Items, int TotalCount)> GetPaginatedDrillStatsByPracticeLogIdAsync(
        int practiceLogId,
        int page = 1,
        int pageSize = 10,
        bool sortByNewest = true,
        TrainingType? type = null)
    {
        try
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
        
            // Create base query
            IQueryable<DrillStatsDao> query;
            if (type != null)
            {
                query = context.DrillStats
                    .Where(d => d.PracticeLogId == practiceLogId && d.TrainingType == (int)type);
            }
            else
            {
                query = context.DrillStats
                    .Where(d => d.PracticeLogId == practiceLogId);
            }

            // Get total count for pagination info
            var totalCount = await query.CountAsync();
        
            // Apply sorting
            query = sortByNewest ? query.OrderByDescending(d => d.StartTime ?? DateTime.MinValue) : query.OrderBy(d => d.Id);
        
            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (items, totalCount);
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return ([], 0);
        }
    }

    public async Task<DrillStatsDao?> CreateDrillStatAsync(DrillStatsDao drillStat)
    {
        try
        {
            // Initialize KeyEventsJson if null
            if (drillStat.KeyEventsJson == null)
            {
                drillStat.KeyEventsJson = new Queue<KeyEventDao>();
            }
            else
            {
                // Convert all DateTime values in KeyEventsJson to UTC
                var convertedEvents = new Queue<KeyEventDao>();
                foreach (var keyEvent in drillStat.KeyEventsJson)
                {
                    keyEvent.Key = SanitizeChar(keyEvent.Key);

                    keyEvent.KeyDownTime = keyEvent.KeyDownTime.Kind != DateTimeKind.Utc 
                        ? DateTime.SpecifyKind(keyEvent.KeyDownTime, DateTimeKind.Utc) 
                        : keyEvent.KeyDownTime;
                
                    keyEvent.KeyUpTime = keyEvent.KeyUpTime.Kind != DateTimeKind.Utc 
                        ? DateTime.SpecifyKind(keyEvent.KeyUpTime, DateTimeKind.Utc) 
                        : keyEvent.KeyUpTime;
                
                    convertedEvents.Enqueue(keyEvent);
                }
                drillStat.KeyEventsJson = convertedEvents;
            }

            // Convert StartTime and FinishTime to UTC if they are set
            if (drillStat.StartTime.HasValue)
            {
                drillStat.StartTime = drillStat.StartTime.Value.Kind != DateTimeKind.Utc
                    ? DateTime.SpecifyKind(drillStat.StartTime.Value, DateTimeKind.Utc)
                    : drillStat.StartTime;
            }

            if (drillStat.FinishTime.HasValue)
            {
                drillStat.FinishTime = drillStat.FinishTime.Value.Kind != DateTimeKind.Utc
                    ? DateTime.SpecifyKind(drillStat.FinishTime.Value, DateTimeKind.Utc)
                    : drillStat.FinishTime;
            }

            // Sanitize string properties that might contain Unicode escape sequences
            if (drillStat.PracticeText != null)
            {
                drillStat.PracticeText = SanitizeString(drillStat.PracticeText);
            }
            drillStat.TypedText = SanitizeString(drillStat.TypedText);

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

    public async Task<DrillStatsDao?> UpdateDrillStatAsync(DrillStatsDao drillStat)
    {
        try
        {
            // Check if drill stat exists
            var existingDrillStat = await context.DrillStats
                .FirstOrDefaultAsync(d => d.Id == drillStat.Id);

            if (existingDrillStat == null)
            {
                ProcessResult.AddError($"Drill stat with ID {drillStat.Id} not found");
                return null;
            }

            // Update properties
            context.Entry(existingDrillStat).CurrentValues.SetValues(drillStat);

            // Update KeyEventsJson separately since it's a complex type
            existingDrillStat.KeyEventsJson = drillStat.KeyEventsJson;

            await context.SaveChangesAsync();
            return existingDrillStat;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<bool> DeleteDrillStatAsync(int id)
    {
        try
        {
            var drillStat = await context.DrillStats.FindAsync(id);
            if (drillStat == null)
            {
                ProcessResult.AddError($"Drill stat with ID {id} not found");
                return false;
            }

            context.DrillStats.Remove(drillStat);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return false;
        }
    }

    public ProcessResult ProcessResult { get; set; } = new(logger);

    /// <summary>
    /// Sanitizes a string to prevent PostgreSQL Unicode escape sequence errors.
    /// PostgreSQL throws errors on backslashes followed by Unicode escape patterns.
    /// </summary>
    private static string SanitizeString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Replace problematic escape sequences
        // The main issue is with \u and \U which PostgreSQL interprets as Unicode escape sequences
        // We need to escape the backslash by replacing \ with \\
        return input.Replace("\\", "\\\\");
    }

    private static char SanitizeChar(char c)
    {
        // Replace null characters or other control characters that cause problems
        if (c == '\0' || char.IsControl(c))
        {
            // Replace with a safe character or space
            return ' ';
        }
        return c;
    }
}