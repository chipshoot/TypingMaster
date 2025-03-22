using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface ILoginLogService
{
    Task<IEnumerable<LoginLog>> GetLoginLogsByAccountIdAsync(int accountId);

    Task<LoginLog> CreateLoginLogAsync(int accountId, string? ipAddress, string? userAgent, bool isSuccessful, string? failureReason = null);

    Task<IEnumerable<LoginLog>> GetRecentLoginLogsAsync(int count);
}