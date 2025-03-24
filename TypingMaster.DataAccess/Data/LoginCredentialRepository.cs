using Microsoft.EntityFrameworkCore;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class LoginCredentialRepository : ILoginCredentialRepository
{
    private readonly ApplicationDbContext _context;

    public LoginCredentialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LoginCredentialDao?> GetByAccountIdAsync(int accountId)
    {
        try
        {
            return await _context.LoginCredentials
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<LoginCredentialDao?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.LoginCredentials
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<LoginCredentialDao> CreateAsync(LoginCredentialDao credential)
    {
        try
        {
            // Set datetime fields
            credential.CreatedAt = DateTime.UtcNow;
            credential.LastUpdated = DateTime.UtcNow;

            _context.LoginCredentials.Add(credential);
            await _context.SaveChangesAsync();
            return credential;
        }
        catch (Exception)
        {
            // Return original if failed
            return credential;
        }
    }

    public async Task<LoginCredentialDao?> UpdateAsync(LoginCredentialDao credential)
    {
        try
        {
            // Update last modified timestamp
            credential.LastUpdated = DateTime.UtcNow;

            _context.LoginCredentials.Update(credential);
            await _context.SaveChangesAsync();
            return credential;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            _context.LoginCredentials.Remove(credential);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateLastLoginAsync(int id, DateTime lastLoginAt)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.LastLoginAt = lastLoginAt;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SetAccountStatusAsync(int id, bool isActive)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.IsActive = isActive;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}