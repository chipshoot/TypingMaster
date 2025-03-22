using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class AccountRepository(ApplicationDbContext context,
    IUserProfileRepository userProfileRepository, 
    Serilog.ILogger logger) 
    : RepositoryBase(logger), IAccountRepository
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
            // Ensure the account exists
            var existingAccount = await context.Accounts
                .Include(a => a.User)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            if (existingAccount == null)
            {
                ProcessResult.AddError($"Account with ID {account.Id} not found");
                return null;
            }

            // Update scalar properties
            context.Entry(existingAccount).CurrentValues.SetValues(account);

            // Update User if needed
            if (account.User != null)
            {
                if (existingAccount.User != null)
                {
                    // Delegate user profile update to the user profile repository
                    var updatedUserProfile = await userProfileRepository.UpdateUserProfileAsync(account.User);
                    if (updatedUserProfile == null)
                    {
                        // Transfer any errors from the user profile repository
                        if (userProfileRepository.ProcessResult.HasErrors())
                        {
                            ProcessResult.PropagandaResult(userProfileRepository.ProcessResult);
                        }
                        return null;
                    }
                }
                else
                {
                    // Create new user profile
                    existingAccount.User = await userProfileRepository.CreateUserProfileAsync(account.User);
                }
            }

            // Update History if needed
            if (account.History != null)
            {
                if (existingAccount.History != null)
                {
                    // Update existing History properties except Id
                    var historyEntry = context.Entry(existingAccount.History);
                    var historyProperties = historyEntry.Properties.Where(p => !p.Metadata.IsPrimaryKey());
                    foreach (var property in historyProperties)
                    {
                        var propertyInfo = typeof(PracticeLogDao).GetProperty(property.Metadata.Name);
                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            var newValue = propertyInfo.GetValue(account.History);
                            property.CurrentValue = newValue;
                        }
                    }
                }
                else
                {
                    // Add new History
                    existingAccount.History = account.History;
                }
            }

            // Save changes
            await context.SaveChangesAsync();
            return existingAccount;
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