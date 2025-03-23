using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.DataAccess.Data;

public interface IAccountRepository
{
    Task<IEnumerable<AccountDao>> GetAllAccountsAsync();

    Task<AccountDao?> GetAccountByIdAsync(int id);

    Task<AccountDao?> GetAccountByEmailAsync(string email);

    Task<AccountDao?> CreateAccountAsync(AccountDao account);

    Task<AccountDao?> UpdateAccountAsync(AccountDao account);

    Task<bool> DeleteAccountAsync(int id);

    /// <summary>
    /// Add drill statistics to an account's practice history
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="drillStat">The drill statistics to add</param>
    /// <returns>The updated account if successful, null otherwise</returns>
    Task<AccountDao?> AddDrillStatToAccountHistoryAsync(int accountId, DrillStatsDao drillStat);

    ProcessResult ProcessResult { get; protected set; }
}