namespace TypingMaster.Server.Dao;

public class UserProfileDao
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string LastName { get; set; } = null!;

    public string? Title { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
}