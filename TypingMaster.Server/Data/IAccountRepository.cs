using TypingMaster.Business.Models;

namespace TypingMaster.Server.Data;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAccountsAsync();

    Task<Account?> GetAccountByIdAsync(int id);

    Task<Account?> GetAccountByEmailAsync(string email);

    Task<Account?> CreateAccountAsync(Account account);

    Task<Account?> UpdateAccountAsync(Account account);

    Task<bool> DeleteAccountAsync(int id);
}