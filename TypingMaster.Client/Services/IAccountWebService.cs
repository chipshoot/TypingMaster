using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IAccountWebService
    {
        Task<Account?> GetAccountAsync(int accountId);

        Task<bool> UpdateAccountAsync(Account account);

        Task<Account?> GetGuestAccount();
    }
}
