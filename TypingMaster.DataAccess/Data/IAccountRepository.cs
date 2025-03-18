using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public interface IAccountRepository
{
    Task<IEnumerable<AccountDao>> GetAllAccountsAsync();

    Task<AccountDao?> GetAccountByIdAsync(int id);

    Task<AccountDao?> GetAccountByEmailAsync(string email);

    Task<AccountDao?> CreateAccountAsync(AccountDao account);

    Task<AccountDao?> UpdateAccountAsync(AccountDao account);

    Task<bool> DeleteAccountAsync(int id);
}