using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class LoginLogRepository : ILoginLogRepository
{
    private readonly ApplicationDbContext _context;

    public LoginLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LoginLogDao>> GetLoginLogsByAccountIdAsync(int accountId)
    {
        try
        {
            return await _context.LoginLogs
                .Where(log => log.AccountId == accountId)
                .OrderByDescending(log => log.LoginTime)
                .ToListAsync();
        }
        catch (Exception)
        {
            return new List<LoginLogDao>();
        }
    }

    public async Task<LoginLogDao> CreateLoginLogAsync(LoginLogDao loginLog)
    {
        try
        {
            _context.LoginLogs.Add(loginLog);
            await _context.SaveChangesAsync();
            return loginLog;
        }
        catch (Exception)
        {
            return loginLog; // Return original object if save fails
        }
    }

    public async Task<IEnumerable<LoginLogDao>> GetRecentLoginLogsAsync(int count)
    {
        try
        {
            return await _context.LoginLogs
                .OrderByDescending(log => log.LoginTime)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception)
        {
            return new List<LoginLogDao>();
        }
    }
}