namespace TypingMaster.DataAccess.Dao;

public class LoginCredentialDao
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public bool IsEmailConfirmed { get; set; }

    public string? ConfirmationToken { get; set; }

    public DateTime? ConfirmationTokenExpiry { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordTokenExpiry { get; set; }

    public bool IsLocked { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    public string? ExternalIdpId { get; set; }

    public string? ExternalIdpType { get; set; }

    // Navigation property
    public AccountDao? Account { get; set; }
}