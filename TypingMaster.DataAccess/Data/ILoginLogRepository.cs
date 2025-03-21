using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public interface ILoginLogRepository
{
    Task<IEnumerable<LoginLogDao>> GetLoginLogsByAccountIdAsync(int accountId);

    Task<LoginLogDao> CreateLoginLogAsync(LoginLogDao loginLog);

    Task<IEnumerable<LoginLogDao>> GetRecentLoginLogsAsync(int count);
}