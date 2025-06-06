
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IAccountClientService
    {
        Task<Account?> GetGuestAccount();

        Task UpdateAccountWithCourseAsync(ApplicationContext applicationState);

    }
}
