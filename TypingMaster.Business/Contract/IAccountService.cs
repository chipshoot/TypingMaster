using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface IAccountService
    {
        Task<Account?> GetAccount(int id);

        Task<Account?> CreateAccount(Account? account);

        Task UpdateAccount(Account account);

        Task<bool> IsAccountUpdated(int accountId, int version);
    }
}