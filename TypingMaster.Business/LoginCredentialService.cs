using AutoMapper;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class LoginCredentialService : ServiceBase, ILoginCredentialService
{
    private readonly ILoginCredentialRepository _repository;
    private readonly IMapper _mapper;
    private readonly int _maxFailedAttempts = 5;
    private readonly TimeSpan _lockoutDuration = TimeSpan.FromMinutes(30);

    public LoginCredentialService(ILoginCredentialRepository repository, IMapper mapper, ILogger logger)
        : base(logger)
    {
        _repository = repository;
        _mapper = mapper;
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

    public async Task<LoginCredential> CreateAsync(int accountId, string email, string password)
    {
        try
        {
            // Hash the password
            var (hash, salt) = PasswordHasher.HashPassword(password);

            // Create the credential
            var credential = new LoginCredentialDao
            {
                AccountId = accountId,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsEmailConfirmed = false, // Requires confirmation
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

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        try
        {
            var credentialDao = await _repository.GetByEmailAsync(email);
            if (credentialDao == null)
            {
                return false;
            }

            // Check if account is locked
            if (credentialDao.IsLocked && credentialDao.LockoutEnd.HasValue && credentialDao.LockoutEnd > DateTime.UtcNow)
            {
                ProcessResult.AddError($"Account is locked until {credentialDao.LockoutEnd}");
                return false;
            }

            // Verify password
            var isPasswordValid = PasswordHasher.VerifyPassword(
                password,
                credentialDao.PasswordHash,
                credentialDao.PasswordSalt);

            if (isPasswordValid)
            {
                // Reset failed login attempts on success
                if (credentialDao.FailedLoginAttempts > 0)
                {
                    await _repository.UpdateFailedLoginAttemptAsync(credentialDao.Id, 0);
                }
                return true;
            }
            else
            {
                // Increment failed attempts on failure
                await HandleFailedLoginAttemptAsync(email);
                return false;
            }
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return false;
            }

            // Verify current password
            var isCurrentPasswordValid = PasswordHasher.VerifyPassword(
                currentPassword,
                credentialDao.PasswordHash,
                credentialDao.PasswordSalt);

            if (!isCurrentPasswordValid)
            {
                ProcessResult.AddError("Current password is incorrect");
                return false;
            }

            // Hash the new password
            var (hash, salt) = PasswordHasher.HashPassword(newPassword);

            // Update the password
            var success = await _repository.UpdatePasswordAsync(credentialDao.Id, hash, salt);
            if (success)
            {
                ProcessResult.AddSuccess();
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        try
        {
            var credentialDao = await _repository.GetByEmailAsync(email);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return string.Empty;
            }

            // Generate a secure token
            var token = PasswordHasher.GenerateSecureToken();

            // Update credential with token
            credentialDao.ResetPasswordToken = token;
            credentialDao.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours

            var updated = await _repository.UpdateAsync(credentialDao);
            if (updated == null)
            {
                ProcessResult.AddError("Failed to update reset token");
                return string.Empty;
            }

            return token;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return string.Empty;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            var credentialDao = await _repository.GetByResetTokenAsync(token);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Invalid or expired reset token");
                return false;
            }

            // Hash the new password
            var (hash, salt) = PasswordHasher.HashPassword(newPassword);

            // Update the password
            var success = await _repository.UpdatePasswordAsync(credentialDao.Id, hash, salt);
            if (success)
            {
                ProcessResult.AddSuccess();
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(int accountId)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return string.Empty;
            }

            // Generate a secure token
            var token = PasswordHasher.GenerateSecureToken();

            // Update credential with token
            credentialDao.ConfirmationToken = token;
            credentialDao.ConfirmationTokenExpiry = DateTime.UtcNow.AddDays(7); // Token valid for 7 days

            var updated = await _repository.UpdateAsync(credentialDao);
            if (updated == null)
            {
                ProcessResult.AddError("Failed to update confirmation token");
                return string.Empty;
            }

            return token;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return string.Empty;
        }
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        try
        {
            var credentialDao = await _repository.GetByConfirmationTokenAsync(token);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Invalid or expired confirmation token");
                return false;
            }

            var success = await _repository.ConfirmEmailAsync(credentialDao.Id);
            if (success)
            {
                ProcessResult.AddSuccess();
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
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

            var updated = await _repository.UpdateAsync(credentialDao);
            return updated != null;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> HandleFailedLoginAttemptAsync(string email)
    {
        try
        {
            var credentialDao = await _repository.GetByEmailAsync(email);
            if (credentialDao == null)
            {
                // Don't reveal account existence
                return false;
            }

            var newFailedAttempts = credentialDao.FailedLoginAttempts + 1;

            // Lock account if max attempts reached
            if (newFailedAttempts >= _maxFailedAttempts)
            {
                var lockoutEnd = DateTime.UtcNow.Add(_lockoutDuration);
                await _repository.LockAccountAsync(credentialDao.Id, lockoutEnd);
                ProcessResult.AddError($"Account locked until {lockoutEnd}");
                return true;
            }
            else
            {
                return await _repository.UpdateFailedLoginAttemptAsync(credentialDao.Id, newFailedAttempts);
            }
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> UnlockAccountAsync(int accountId)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return false;
            }

            var success = await _repository.UnlockAccountAsync(credentialDao.Id);
            if (success)
            {
                ProcessResult.AddSuccess("Account unlocked successfully");
            }
            return success;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> IsAccountLockedAsync(string email)
    {
        try
        {
            var credentialDao = await _repository.GetByEmailAsync(email);
            if (credentialDao == null)
            {
                return false;
            }

            return credentialDao.IsLocked &&
                   credentialDao.LockoutEnd.HasValue &&
                   credentialDao.LockoutEnd > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int accountId)
    {
        try
        {
            var credentialDao = await _repository.GetByAccountIdAsync(accountId);
            if (credentialDao == null)
            {
                ProcessResult.AddError("Account not found");
                return false;
            }

            return await _repository.DeleteAsync(credentialDao.Id);
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }
}