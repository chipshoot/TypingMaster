using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TypingMaster.Business.Models;
using TypingMaster.Server.Dao;

namespace TypingMaster.Server.Data;

public class AccountRepository(ApplicationDbContext context, IMapper mapper, Serilog.ILogger logger) : RepositoryBase(logger), IAccountRepository
{
    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        try
        {
            var accountDaos= await context.Accounts
                .Include(a => a.User)
                .Include(a => a.History)
                .ToListAsync();
            var account = mapper.Map<IEnumerable<Account>>(accountDaos);
            return account;
        }
        catch (Exception e)
        {
            ProcessResult.AddError(e.Message);
            return new List<Account>();
        }
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        try
        {
            var accountDao = await context.Accounts
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.Id == id);
            var account = mapper.Map<Account>(accountDao);
            return account;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<Account?> GetAccountByEmailAsync(string email)
    {
        try
        {
            var accountDao = await context.Accounts
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
            var account = mapper.Map<Account>(accountDao);
            return account;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<Account?> CreateAccountAsync(Account account)
    {
        try
        {
            var accountDao = mapper.Map<AccountDao>(account);
            context.Accounts.Add(accountDao);
            await context.SaveChangesAsync();
            return account;

        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<Account?> UpdateAccountAsync(Account account)
    {
        try
        {
            var accountDao = mapper.Map<AccountDao>(account);
            context.Entry(accountDao).State = EntityState.Modified;
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