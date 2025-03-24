using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class AccountService(IAccountRepository accountRepository, IMapper mapper, ILogger logger, ICourseService courseService) : ServiceBase(logger), IAccountService
{
    public async Task<IEnumerable<Account>> GetAllAccounts()
    {
        try
        {
            var accountDaos = await accountRepository.GetAllAccountsAsync();
            var accounts = accountDaos.Select(mapper.Map<Account>);
            return accounts;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return new List<Account>();
        }
    }

    public async Task<Account?> GetAccountById(int id)
    {
        try
        {
            var accountDao = await accountRepository.GetAccountByIdAsync(id);
            if (accountDao == null)
            {
                return null;
            }

            var account = mapper.Map<Account>(accountDao);
            return account;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
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
            // Validate course ID in history before saving to database
            if (account.History != null && account.History.CurrentCourseId != Guid.Empty)
            {
                var course = await courseService.GetCourse(account.History.CurrentCourseId);
                if (course == null)
                {
                    // Invalid course ID found in history, set to empty GUID
                    account.History.CurrentCourseId = Guid.Empty;
                    logger.Warning("Invalid course ID {CourseId} found in account history. Setting to empty GUID.", account.History.CurrentCourseId);
                }
            }

            var accountDao = mapper.Map<AccountDao>(account);
            var newAccountDao = await accountRepository.CreateAccountAsync(accountDao);
            ProcessResult.AddSuccess();
            var newAccount = mapper.Map<Account>(newAccountDao);
            return newAccount;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<Account?> UpdateAccount(Account? account)
    {
        try
        {
            if (account == null)
            {
                return null;
            }

            var existingAccountDao = await accountRepository.GetAccountByIdAsync(account.Id);
            if (existingAccountDao == null)
            {
                ProcessResult.AddError($"Account with ID {account.Id} not found");
                return null;
            }

            // Check if the account has been updated since this version
            if (existingAccountDao.Version > account.Version)
            {
                ProcessResult.AddError("The account has been modified by another user. Please refresh and try again.");
                return null;
            }

            // Validate course ID in history before updating
            if (account.History != null && account.History.CurrentCourseId != Guid.Empty)
            {
                var course = await courseService.GetCourse(account.History.CurrentCourseId);
                if (course == null)
                {
                    // Invalid course ID found in history, set to empty GUID
                    account.History.CurrentCourseId = Guid.Empty;
                    logger.Warning("Invalid course ID {CourseId} found in account history during update. Setting to empty GUID.", account.History.CurrentCourseId);
                }
            }

            try
            {
                var accountDao = mapper.Map<AccountDao>(account);
                var updatedAccountDao = await accountRepository.UpdateAccountAsync(accountDao);
                if (updatedAccountDao == null)
                {
                    ProcessResult.AddError("Failed to update account");
                    return null;
                }

                var updatedAccount = mapper.Map<Account>(updatedAccountDao);
                return updatedAccount;
            }
            catch (DbUpdateConcurrencyException)
            {
                ProcessResult.AddError("The account has been modified by another user. Please refresh and try again");
                return null;
            }
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }

    public async Task<bool> DeleteAccount(int id)
    {
        try
        {
            var accountDao = await accountRepository.GetAccountByIdAsync(id);
            if (accountDao == null)
            {
                ProcessResult.AddError($"Account with ID {id} not found");
                return false;
            }

            // Set soft delete properties
            accountDao.IsDeleted = true;
            accountDao.DeletedAt = DateTime.UtcNow;

            // Optionally anonymize personal data
            accountDao.AccountEmail = $"deleted-{Guid.NewGuid()}@example.com";

            // Update instead of delete
            await accountRepository.UpdateAccountAsync(accountDao);
            ProcessResult.AddSuccess();
            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> IsAccountUpdated(int accountId, int version)
    {
        try
        {
            var accountDao = await accountRepository.GetAccountByIdAsync(accountId);
            if (accountDao == null)
            {
                // Account doesn't exist
                ProcessResult.AddError($"Account with ID {accountId} not found");
                return false;
            }

            // Return true if the current version is greater than the provided version
            // This means the account has been updated since the version being checked
            return accountDao.Version > version;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            // In case of error, assume the account has been updated to be safe
            return true;
        }
    }

    public Account GetGuestAccount()
    {
        var guest = new Account
        {
            AccountEmail = "guest@test.com",
            AccountName = Guid.NewGuid().ToString(),
            History = new PracticeLog(),
            User = new UserProfile()
        };

        return guest;
    }

    public async Task<Account?> GetAccountByEmail(string email)
    {
        try
        {
            var accountDao = await accountRepository.GetAccountByEmailAsync(email);
            if (accountDao == null)
            {
                return null;
            }

            var account = mapper.Map<Account>(accountDao);
            return account;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }
}