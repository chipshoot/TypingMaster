using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class LoginCredentialService : ServiceBase, ILoginCredentialService
{
    private readonly ILoginCredentialRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoginLogService _loginLogService;

    public LoginCredentialService(
        ILoginCredentialRepository repository,
        IMapper mapper,
        ILoginLogService loginLogService,
        ILogger logger)
        : base(logger)
    {
        _repository = repository;
        _mapper = mapper;
        _loginLogService = loginLogService;
    }

    public async Task<LoginCredential?> GetByAccountIdAsync(int accountId)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            return credentialDao != null ? _mapper.Map<LoginCredential>(credentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<LoginCredential?> GetByEmailAsync(string email)
    {
        try
        {
            var credentialDao = await _repository.GetByEmailAsync(email);
            return credentialDao != null ? _mapper.Map<LoginCredential>(credentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<LoginCredential> CreateAsync(int accountId, string email)
    {
        try
        {
            var credential = new LoginCredentialDao
            {
                AccountId = accountId,
                Email = email,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            var createdCredential = await _repository.CreateAsync(credential);
            return _mapper.Map<LoginCredential>(createdCredential);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            // Return a default credential if creation fails
            return new LoginCredential
            {
                AccountId = accountId,
                Email = email,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    public async Task<LoginCredential?> UpdateAsync(LoginCredential credential)
    {
        try
        {
            var credentialDao = _mapper.Map<LoginCredentialDao>(credential);
            var updatedCredentialDao = await _repository.UpdateAsync(credentialDao);
            return updatedCredentialDao != null ? _mapper.Map<LoginCredential>(updatedCredentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<bool> UpdateExternalIdpInfoAsync(int accountId, string externalIdpId, string externalIdpType)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return false;
            }

            credentialDao.ExternalIdpId = externalIdpId;
            credentialDao.ExternalIdpType = externalIdpType;
            credentialDao.LastUpdated = DateTime.UtcNow;

            var updatedCredentialDao = await _repository.UpdateAsync(credentialDao);
            return updatedCredentialDao != null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> UpdateLastLoginAsync(int accountId)
    {
        try
        {
            var success = await _repository.UpdateLastLoginAsync(accountId, DateTime.UtcNow);
            if (success)
            {
                // Log successful login
                await _loginLogService.CreateLoginLogAsync(
                    accountId,
                    null, // IP address will be set by the API layer
                    null, // User agent will be set by the API layer
                    true,
                    null);
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> SetAccountStatusAsync(int accountId, bool isActive)
    {
        try
        {
            var success = await _repository.SetAccountStatusAsync(accountId, isActive);
            if (success)
            {
                // Log account status change
                await _loginLogService.CreateLoginLogAsync(
                    accountId,
                    null, // IP address will be set by the API layer
                    null, // User agent will be set by the API layer
                    isActive,
                    isActive ? "Account activated" : "Account deactivated");
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }
}