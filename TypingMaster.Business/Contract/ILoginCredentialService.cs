using TypingMaster.Core.Models;

namespace TypingMaster.Business.Contract;

public interface ILoginCredentialService
{
    Task<LoginCredential?> GetByAccountIdAsync(int accountId);

    Task<LoginCredential?> GetByEmailAsync(string email);

    Task<LoginCredential> CreateAsync(int accountId, string email);

    Task<LoginCredential?> UpdateAsync(LoginCredential credential);

    Task<bool> UpdateExternalIdpInfoAsync(int accountId, string externalIdpId, string externalIdpType);

    Task<bool> UpdateLastLoginAsync(int accountId);

    Task<bool> SetAccountStatusAsync(int accountId, bool isActive);
}