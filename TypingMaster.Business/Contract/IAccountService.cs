using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAccounts();

        Task<Account?> GetAccount(int id);

        Task<Account?> CreateAccount(Account? account);

        Task<Account?> UpdateAccount(Account account);

        Task<bool> DeleteAccount(int id);

        Task<bool> IsAccountUpdated(int accountId, int version);

        /// <summary>
        /// Get a guest account.
        /// </summary>
        /// <returns>The guest account temporary used by guest for playing around, when guest registered as membership, the account will convert to normal account </returns>
        Account GetGuestAccount();
    }
}