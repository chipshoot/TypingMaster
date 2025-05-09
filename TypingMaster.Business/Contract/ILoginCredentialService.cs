using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Contract;

public interface ILoginCredentialService
{
    Task<LoginCredential?> GetByAccountId(int accountId);

    Task<LoginCredential?> GetByEmail(string email);

    Task<LoginCredential> Create(int accountId, string email);

    Task<LoginCredential?> Update(LoginCredential credential);

    Task<bool> UpdateExternalIdpInfo(int accountId, string externalIdpId, string externalIdpType);

    Task<bool> UpdateLastLogin(int accountId);

    Task<bool> SetAccountStatus(int accountId, bool isActive);

    /// <summary>
    /// Gets or sets the result of the process, including status, error messages, and additional information.
    /// </summary>
    ProcessResult ProcessResult { get; set; }


}