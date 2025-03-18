using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class AccountRepository(ApplicationDbContext context, Serilog.ILogger logger) : RepositoryBase(logger), IAccountRepository
{
    public async Task<IEnumerable<AccountDao>> GetAllAccountsAsync()
    {
        try
        {
            var accountDaos = await context.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.User)
                .Include(a => a.History)
                .ToListAsync();
            return accountDaos;
        }
        catch (Exception e)
        {
            ProcessResult.AddError(e.Message);
            return new List<AccountDao>();
        }
    }

    public async Task<AccountDao?> GetAccountByIdAsync(int id)
    {
        try
        {
            var accountDao = await context.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.Id == id);
            return accountDao;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<AccountDao?> GetAccountByEmailAsync(string email)
    {
        try
        {
            var accountDao = await context.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
            return accountDao;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<AccountDao?> CreateAccountAsync(AccountDao account)
    {
        try
        {
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
            return account;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<AccountDao?> UpdateAccountAsync(AccountDao account)
    {
        try
        {
            context.Entry(account).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return account;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        try
        {
            var accountDao = await context.Accounts.FindAsync(id);
            if (accountDao == null)
                return false;

            context.Accounts.Remove(accountDao);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return false;
        }
    }
}