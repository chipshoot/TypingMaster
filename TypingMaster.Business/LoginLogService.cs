using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.Business;

public class LoginLogService(ILoginLogRepository loginLogRepository, IMapper mapper, ILogger logger)
    : ILoginLogService
{
    public ProcessResult ProcessResult { get; set; } = new(logger);

    public async Task<IEnumerable<LoginLog>> GetLoginLogsByAccountId(int accountId)
    {
        try
        {
            var loginLogDaos = await loginLogRepository.GetLoginLogsByAccountIdAsync(accountId);
            return loginLogDaos.Select(mapper.Map<LoginLog>);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return Enumerable.Empty<LoginLog>();
        }
    }

    public async Task<LoginLog> CreateLoginLog(int accountId, string? ipAddress, string? userAgent, bool isSuccessful, string? failureReason = null)
    {
        try
        {
            var loginLogDao = new LoginLogDao
            {
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = DateTime.UtcNow,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason
            };

            var createdLoginLogDao = await loginLogRepository.CreateLoginLogAsync(loginLogDao);
            return mapper.Map<LoginLog>(createdLoginLogDao);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return new LoginLog
            {
                AccountId = accountId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = DateTime.UtcNow,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason
            };
        }
    }

    public async Task<IEnumerable<LoginLog>> GetRecentLoginLogs(int count)
    {
        try
        {
            var loginLogDaos = await loginLogRepository.GetRecentLoginLogsAsync(count);
            return loginLogDaos.Select(mapper.Map<LoginLog>);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return Enumerable.Empty<LoginLog>();
        }
    }
}