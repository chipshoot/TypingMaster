using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public interface ILoginCredentialRepository
{
    Task<LoginCredentialDao?> GetByAccountIdAsync(int accountId);

    Task<LoginCredentialDao?> GetByEmailAsync(string email);

    Task<LoginCredentialDao> CreateAsync(LoginCredentialDao credential);

    Task<LoginCredentialDao?> UpdateAsync(LoginCredentialDao credential);

    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateFailedLoginAttemptAsync(int id, int failedAttempts);

    Task<bool> UpdateRefreshTokenAsync(int id, string refreshToken, DateTime expiry);

    Task<bool> UpdatePasswordAsync(int id, string passwordHash, string passwordSalt);

    Task<bool> LockAccountAsync(int id, DateTime lockoutEnd);

    Task<bool> UnlockAccountAsync(int id);

    Task<bool> ConfirmEmailAsync(int id);

    Task<LoginCredentialDao?> GetByResetTokenAsync(string resetToken);

    Task<LoginCredentialDao?> GetByConfirmationTokenAsync(string confirmationToken);
}