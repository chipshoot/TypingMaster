using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class AccountRepository(
    ApplicationDbContext context,
    IUserProfileRepository userProfileRepository,
    IPracticeLogRepository practiceLogRepository,
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
                .Include(a => a.Courses)
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
            // Create the account
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
                        if (userProfileRepository.ProcessResult.HasErrors)
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

            // Note: Practice log updates are handled entirely by the service layer
            // The repository should not perform additional practice log operations
            // as this would cause double updates and potential concurrency issues

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

    public async Task<AccountDao?> AddDrillStatToAccountHistoryAsync(int accountId, DrillStatsDao drillStat)
    {
        try
        {
            // Get the account with its history
            var account = await context.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.History)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                ProcessResult.AddError($"Account with ID {accountId} not found");
                return null;
            }

            // Check if account has history
            if (account.History == null)
            {
                ProcessResult.AddError($"Account with ID {accountId} does not have a practice log history");
                return null;
            }

            // Add the drill stat to the practice log
            var addedDrillStat = await practiceLogRepository.AddDrillStatAsync(account.History.Id, drillStat);
            if (addedDrillStat == null)
            {
                // Transfer errors from practice log repository
                if (practiceLogRepository.ProcessResult.HasErrors)
                {
                    ProcessResult.PropagandaResult(practiceLogRepository.ProcessResult);
                }
                return null;
            }

            // Get the updated practice log
            var updatedPracticeLog = await practiceLogRepository.GetPracticeLogByIdAsync(account.History.Id);
            if (updatedPracticeLog != null)
            {
                account.History = updatedPracticeLog;
            }

            return account;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }
}