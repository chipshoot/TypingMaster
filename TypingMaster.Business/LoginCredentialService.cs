using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class LoginCredentialService(
    ILoginCredentialRepository repository,
    IMapper mapper,
    ILoginLogService loginLogService,
    ILogger logger)
    : ILoginCredentialService
{
    public ProcessResult ProcessResult { get; set; } = new(logger);

    public async Task<LoginCredential?> GetByAccountId(int accountId)
    {
        try
        {
            var credentialDao = await repository.GetByAccountIdAsync(accountId);
            return credentialDao != null ? mapper.Map<LoginCredential>(credentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<LoginCredential?> GetByEmail(string email)
    {
        try
        {
            var credentialDao = await repository.GetByEmailAsync(email);
            return credentialDao != null ? mapper.Map<LoginCredential>(credentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<LoginCredential> Create(int accountId, string email)
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

            var createdCredential = await repository.CreateAsync(credential);
            return mapper.Map<LoginCredential>(createdCredential);
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

    public async Task<LoginCredential?> Update(LoginCredential credential)
    {
        try
        {
            var credentialDao = mapper.Map<LoginCredentialDao>(credential);
            var updatedCredentialDao = await repository.UpdateAsync(credentialDao);
            return updatedCredentialDao != null ? mapper.Map<LoginCredential>(updatedCredentialDao) : null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<bool> UpdateExternalIdpInfo(int accountId, string externalIdpId, string externalIdpType)
    {
        try
        {
            var credentialDao = await repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return false;
            }

            credentialDao.ExternalIdpId = externalIdpId;
            credentialDao.ExternalIdpType = externalIdpType;
            credentialDao.LastUpdated = DateTime.UtcNow;

            var updatedCredentialDao = await repository.UpdateAsync(credentialDao);
            return updatedCredentialDao != null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> UpdateLastLogin(int accountId)
    {
        try
        {
            var success = await repository.UpdateLastLoginAsync(accountId, DateTime.UtcNow);
            if (success)
            {
                // Log successful login
                await loginLogService.CreateLoginLog(
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

    public async Task<bool> SetAccountStatus(int accountId, bool isActive)
    {
        try
        {
            var success = await repository.SetAccountStatusAsync(accountId, isActive);
            if (success)
            {
                // Log account status change
                await loginLogService.CreateLoginLog(
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