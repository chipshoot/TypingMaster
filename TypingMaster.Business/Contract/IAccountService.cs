using TypingMaster.Business.Models;
using TypingMaster.Business.Utility;

namespace TypingMaster.Business.Contract
{
    public interface IAccountService
    {
        ProcessResult ProcessResult { get; set; }

        Task<IEnumerable<Account>> GetAllAccounts();

        Task<Account?> GetAccountById(int id);

        Task<Account?> GetAccountByEmail(string email);

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