using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;

namespace TypingMaster.Business;

public class AccountService(
    IAccountRepository accountRepository,
    IPracticeLogService practiceLogService,
    ICourseService courseService,
    IMapper mapper,
    ILogger logger) : IAccountService
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
            ProcessResult.AddError(TypingMasterConstants.InvalidAccountData);
            return null;
        }

        try
        {
            // Initialize practice log for new account if not provided
            if (account.History == null)
            {
                account.History = new PracticeLog();
            }
            else if (account.History.Id != 0)
            {
                ProcessResult.AddError("Cannot create a new account with an existing practice log");
                return null;
            }

            // Validate course ID in history before saving to database
            if (account.History.CurrentCourseId != Guid.Empty)
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
            if (newAccountDao == null)
            {
                ProcessResult.AddError("Failed to create account");
                return null;
            }

            // Update the practice log with the new account ID
            account.History = new PracticeLog { AccountId = newAccountDao.Id };
            var practiceLog = await practiceLogService.CreatePracticeLog(account.History);
            if (practiceLog == null)
            {
                ProcessResult.AddError("Failed to create practice log");
                return null;
            }

            // Update the account with the created practice log
            newAccountDao.History = mapper.Map<PracticeLogDao>(practiceLog);
            var updatedAccountDao = await accountRepository.UpdateAccountAsync(newAccountDao);
            if (updatedAccountDao == null)
            {
                ProcessResult.AddError("Failed to update account with practice log");
                return null;
            }

            ProcessResult.AddSuccess();
            return mapper.Map<Account>(updatedAccountDao);
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
                // Update or create practice log if it exists
                if (account.History != null)
                {
                    // Check if practice log exists
                    PracticeLog? practiceLogSaved;
                    var existingPracticeLog = await practiceLogService.GetPracticeLogByAccountId(account.Id);
                    if (existingPracticeLog != null)
                    {
                        // Update existing practice log
                        account.History.Id = existingPracticeLog.Id;
                        account.History.AccountId = account.Id;
                        practiceLogSaved = await practiceLogService.UpdatePracticeLog(account.History);
                    }
                    else
                    {
                        // Create new practice log
                        practiceLogSaved = await practiceLogService.CreatePracticeLog(account.History);
                    }

                    if (practiceLogSaved == null)
                    {
                        ProcessResult.AddError("Failed to update/create practice log");
                        return null;
                    }

                    // Map the account to DAO (without the history to avoid conflicts)
                    var accountToUpdate = new Account
                    {
                        Id = account.Id,
                        AccountName = account.AccountName,
                        AccountEmail = account.AccountEmail,
                        GoalStats = account.GoalStats,
                        Version = account.Version,
                        User = account.User,
                        // Don't set History here - let the repository handle the existing relationship
                    };

                    var accountDao = mapper.Map<AccountDao>(accountToUpdate);

                    var updatedAccountDao = await accountRepository.UpdateAccountAsync(accountDao);
                    if (updatedAccountDao == null)
                    {
                        ProcessResult.AddError("Failed to update account");
                        return null;
                    }

                    // Get the final account with updated practice log
                    var finalAccountDao = await accountRepository.GetAccountByIdAsync(account.Id);
                    if (finalAccountDao == null)
                    {
                        ProcessResult.AddError("Failed to retrieve updated account");
                        return null;
                    }

                    var updatedAccount = mapper.Map<Account>(finalAccountDao);
                    return updatedAccount;
                }
                else
                {
                    // No practice log to update, just update the account
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
            Id = TypingMasterConstants.GuestAccountId,
            AccountEmail = "guest@test.com",
            AccountName = Guid.NewGuid().ToString(),
            History = new PracticeLog(),
            User = new UserProfile()
        };

        return guest;
    }

    public async Task<bool> SetAccountStatus(int accountId, bool isActive)
    {
        try
        {
            var accountDao = await accountRepository.GetAccountByIdAsync(accountId);
            if (accountDao == null)
            {
                ProcessResult.AddError($"Account with ID {accountId} not found");
                return false;
            }

            // Update the account status
            accountDao.IsDeleted = !isActive;
            if (accountDao.IsDeleted)
            {
                accountDao.DeletedAt = DateTime.UtcNow;
            }

            var updatedAccountDao = await accountRepository.UpdateAccountAsync(accountDao);
            if (updatedAccountDao == null)
            {
                ProcessResult.AddError("Failed to update account status");
                return false;
            }

            ProcessResult.AddSuccess();
            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
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

    public ProcessResult ProcessResult { get; set; } = new(logger);
}