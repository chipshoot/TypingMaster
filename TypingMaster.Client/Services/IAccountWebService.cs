using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IAccountWebService
    {
        Task<Account?> GetAccountAsync(int accountId);

        Task<AccountResponse> UpdateAccountAsync(Account account);

        Task<Account?> GetGuestAccount();
    }
}
