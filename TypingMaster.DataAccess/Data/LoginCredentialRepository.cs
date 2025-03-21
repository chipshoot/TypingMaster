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

    public async Task<bool> UpdateFailedLoginAttemptAsync(int id, int failedAttempts)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.FailedLoginAttempts = failedAttempts;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateRefreshTokenAsync(int id, string refreshToken, DateTime expiry)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.RefreshToken = refreshToken;
            credential.RefreshTokenExpiry = expiry;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdatePasswordAsync(int id, string passwordHash, string passwordSalt)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.PasswordHash = passwordHash;
            credential.PasswordSalt = passwordSalt;
            credential.LastUpdated = DateTime.UtcNow;

            // Reset any password reset tokens
            credential.ResetPasswordToken = null;
            credential.ResetPasswordTokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> LockAccountAsync(int id, DateTime lockoutEnd)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.IsLocked = true;
            credential.LockoutEnd = lockoutEnd;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UnlockAccountAsync(int id)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.IsLocked = false;
            credential.LockoutEnd = null;
            credential.FailedLoginAttempts = 0;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ConfirmEmailAsync(int id)
    {
        try
        {
            var credential = await _context.LoginCredentials.FindAsync(id);
            if (credential == null)
            {
                return false;
            }

            credential.IsEmailConfirmed = true;
            credential.ConfirmationToken = null;
            credential.ConfirmationTokenExpiry = null;
            credential.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<LoginCredentialDao?> GetByResetTokenAsync(string resetToken)
    {
        try
        {
            return await _context.LoginCredentials
                .FirstOrDefaultAsync(c => c.ResetPasswordToken == resetToken &&
                                           c.ResetPasswordTokenExpiry > DateTime.UtcNow);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<LoginCredentialDao?> GetByConfirmationTokenAsync(string confirmationToken)
    {
        try
        {
            return await _context.LoginCredentials
                .FirstOrDefaultAsync(c => c.ConfirmationToken == confirmationToken &&
                                           c.ConfirmationTokenExpiry > DateTime.UtcNow);
        }
        catch (Exception)
        {
            return null;
        }
    }
}