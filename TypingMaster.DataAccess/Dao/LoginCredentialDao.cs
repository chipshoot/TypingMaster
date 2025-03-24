namespace TypingMaster.DataAccess.Dao;

public class LoginCredentialDao
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string? ExternalIdpId { get; set; }

    public string? ExternalIdpType { get; set; }

    public DateTime LastLoginAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    // Navigation property
    public AccountDao? Account { get; set; }
}