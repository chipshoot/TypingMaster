using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract;

public interface ILoginCredentialService
{
    Task<LoginCredential?> GetByAccountIdAsync(int accountId);

    Task<LoginCredential?> GetByEmailAsync(string email);

    Task<LoginCredential> CreateAsync(int accountId, string email, string password);

    Task<LoginCredential?> UpdateAsync(LoginCredential credential);

    Task<bool> ValidateCredentialsAsync(string email, string password);

    Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword);

    Task<string> GeneratePasswordResetTokenAsync(string email);

    Task<bool> ResetPasswordAsync(string token, string newPassword);

    Task<string> GenerateEmailConfirmationTokenAsync(int accountId);

    Task<bool> ConfirmEmailAsync(string token);

    Task<bool> UpdateExternalIdpInfoAsync(int accountId, string externalIdpId, string externalIdpType);

    Task<bool> HandleFailedLoginAttemptAsync(string email);

    Task<bool> UnlockAccountAsync(int accountId);

    Task<bool> IsAccountLockedAsync(string email);

    Task<bool> DeleteAsync(int accountId);
}