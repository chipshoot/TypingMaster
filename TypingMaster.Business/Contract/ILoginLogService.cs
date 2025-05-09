using TypingMaster.Core.Models;

namespace TypingMaster.Business.Contract;

public interface ILoginLogService
{
    Task<IEnumerable<LoginLog>> GetLoginLogsByAccountId(int accountId);

    Task<LoginLog> CreateLoginLog(int accountId, string? ipAddress, string? userAgent, bool isSuccessful, string? failureReason = null);

    Task<IEnumerable<LoginLog>> GetRecentLoginLogs(int count);
}