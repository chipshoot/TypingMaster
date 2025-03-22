using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class LoginLogService : ServiceBase, ILoginLogService
{
    private readonly ILoginLogRepository _loginLogRepository;
    private readonly IMapper _mapper;

    public LoginLogService(ILoginLogRepository loginLogRepository, IMapper mapper, ILogger logger)
        : base(logger)
    {
        _loginLogRepository = loginLogRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoginLog>> GetLoginLogsByAccountIdAsync(int accountId)
    {
        try
        {
            var loginLogDaos = await _loginLogRepository.GetLoginLogsByAccountIdAsync(accountId);
            return loginLogDaos.Select(_mapper.Map<LoginLog>);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return Enumerable.Empty<LoginLog>();
        }
    }

    public async Task<LoginLog> CreateLoginLogAsync(int accountId, string? ipAddress, string? userAgent, bool isSuccessful, string? failureReason = null)
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

            var createdLoginLogDao = await _loginLogRepository.CreateLoginLogAsync(loginLogDao);
            return _mapper.Map<LoginLog>(createdLoginLogDao);
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

    public async Task<IEnumerable<LoginLog>> GetRecentLoginLogsAsync(int count)
    {
        try
        {
            var loginLogDaos = await _loginLogRepository.GetRecentLoginLogsAsync(count);
            return loginLogDaos.Select(_mapper.Map<LoginLog>);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return Enumerable.Empty<LoginLog>();
        }
    }
}