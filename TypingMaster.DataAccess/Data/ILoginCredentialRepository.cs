using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public interface ILoginCredentialRepository
{
    Task<LoginCredentialDao?> GetByAccountIdAsync(int accountId);

    Task<LoginCredentialDao?> GetByEmailAsync(string email);

    Task<LoginCredentialDao> CreateAsync(LoginCredentialDao credential);

    Task<LoginCredentialDao?> UpdateAsync(LoginCredentialDao credential);

    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateLastLoginAsync(int id, DateTime lastLoginAt);

    Task<bool> SetAccountStatusAsync(int id, bool isActive);
}