using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class AccountService(ILogger logger) : ServiceBase(logger), IAccountService
{
    public async Task<Account?> GetAccount(int id)
    {
        var savedAcc = new Account();
        return savedAcc;
    }

    public async Task<Account?> CreateAccount(Account? account)
    {
        if (account == null || string.IsNullOrEmpty(account.AccountName) || string.IsNullOrEmpty(account.AccountEmail))
        {
            ProcessResult.AddError(InvalidAccountData);
            return null;
        }

        try
        {
            await SaveAccount(account);
            ProcessResult.AddSuccess();
            return account;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task UpdateAccount(Account? account)
    {
        if (account == null || string.IsNullOrEmpty(account.AccountName) || string.IsNullOrEmpty(account.AccountEmail))
        {
            ProcessResult.AddError(InvalidAccountData);
            return;
        }

        try
        {
            await SaveAccount(account);
            ProcessResult.AddSuccess();
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
        }
    }

    public Task<bool> IsAccountUpdated(int accountId, int version)
    {
        return Task.FromResult(true);
    }

    private async Task SaveAccount(Account account)
    {
    }
}