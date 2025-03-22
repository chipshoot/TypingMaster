using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class UserProfileRepository(ApplicationDbContext context, Serilog.ILogger logger) 
    : RepositoryBase(logger), IUserProfileRepository
{
    public async Task<UserProfileDao?> GetUserProfileByIdAsync(int id)
    {
        try
        {
            var userProfile = await context.UserProfiles
                .FirstOrDefaultAsync(u => u.Id == id);
            return userProfile;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<UserProfileDao> CreateUserProfileAsync(UserProfileDao userProfile)
    {
        try
        {
            context.UserProfiles.Add(userProfile);
            await context.SaveChangesAsync();
            return userProfile;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }

    public async Task<UserProfileDao?> UpdateUserProfileAsync(UserProfileDao userProfile)
    {
        try
        {
            // Find the existing user profile
            var existingProfile = await context.UserProfiles
                .FirstOrDefaultAsync(u => u.Id == userProfile.Id);
                
            if (existingProfile == null)
            {
                ProcessResult.AddError($"User profile with ID {userProfile.Id} not found");
                return null;
            }

            // Update properties excluding Id
            context.Entry(existingProfile).CurrentValues.SetValues(userProfile);
            
            await context.SaveChangesAsync();
            return existingProfile;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return null;
        }
    }
    public async Task<bool> CanDeleteUserProfileAsync(int id)
    {
        try
        {
            // Check if this user profile exists
            var userProfile = await context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                // Profile doesn't exist, so technically it can be "deleted"
                return true;
            }
        
            // Check if this user profile is linked to any active accounts
            var linkedAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.User.Id == id && !a.IsDeleted);
            
            // Can only delete if no linked active accounts
            return linkedAccount == null;
        }
        catch (Exception e)
        {
            ProcessResult.AddException(e);
            return false;
        }
    }


    public async Task<bool> DeleteUserProfileAsync(int id)
    {
        try
        {
            var userProfile = await context.UserProfiles.FindAsync(id);
            if (userProfile == null)
                return false;

            // Check if this user profile is linked to any active accounts
            var canDelete = await CanDeleteUserProfileAsync(id);
            if (!canDelete)
            {
                // Find the linked account for error message
                var linkedAccount = await context.Accounts
                    .FirstOrDefaultAsync(a => a.User.Id == id && !a.IsDeleted);
                
                ProcessResult.AddError(linkedAccount != null 
                    ? $"Cannot delete user profile (ID: {id}) because it's linked to an active account (ID: {linkedAccount.Id}, Email: {linkedAccount.AccountEmail})"
                    : $"Cannot delete user profile (ID: {id}) because it's linked to active accounts");
                
                return false;
            }

            context.UserProfiles.Remove(userProfile);
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