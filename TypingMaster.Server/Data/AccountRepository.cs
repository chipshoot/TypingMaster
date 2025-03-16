//using Microsoft.EntityFrameworkCore;
//using TypingMaster.Business.Models;

//namespace TypingMaster.Server.Data;

//public class AccountRepository(ApplicationDbContext context, Serilog.ILogger logger) : RepositoryBase(logger), IAccountRepository
//{
////    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
////    {
////        try
////        {
////            return await context.Accounts
////                .Include(a => a.User)
////                .Include(a => a.History)
////                .ToListAsync();
////        }
////        catch (Exception e)
////        {
////            ProcessResult.AddError(e.Message);
////            return new List<Account>();
////        }
////    }

////    public async Task<Account> GetAccountByIdAsync(int id)
////    {
////        try
////        {
////            return await context.Accounts
////                .Include(a => a.User)
////                .Include(a => a.History)
////                .FirstOrDefaultAsync(a => a.Id == id);

////        }
////        catch (Exception e)
////        {
////            ProcessResult.AddException(e);
////            return null;
////        }
////    }

////    public async Task<Account> GetAccountByEmailAsync(string email)
////    {
////        try
////        {
////            return await context.Accounts
////                .Include(a => a.User)
////                .Include(a => a.History)
////                .FirstOrDefaultAsync(a => a.AccountEmail == email);

////        }
////        catch (Exception e)
////        {
////            ProcessResult.AddException(e);
////            return null;
////        }
////    }

////    public async Task<Account> CreateAccountAsync(Account account)
////    {
////        try
////        {
////            context.Accounts.Add(account);
////            await context.SaveChangesAsync();
////            return account;

////        }
////        catch (Exception e)
////        {
////            ProcessResult.AddException(e);
////            return null;
////        }
////    }

////    public async Task<Account> UpdateAccountAsync(Account account)
////    {
////        try
////        {
////            context.Entry(account).State = EntityState.Modified;
////            await context.SaveChangesAsync();
////            return account;
////        }
////        catch (Exception e)
////        {
////            ProcessResult.AddException(e);
////            return null;
////        }
////    }

////    public async Task<bool> DeleteAccountAsync(int id)
////    {
////        try
////        {
////            var account = await context.Accounts.FindAsync(id);
////            if (account == null)
////                return false;

////            context.Accounts.Remove(account);
////            await context.SaveChangesAsync();
////            return true;
////        }
////        catch(Exception e)
////        {
////            ProcessResult.AddException(e);
////            return false;
////        }
////    }
//}